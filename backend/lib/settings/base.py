from typing import (
    Dict,
    Generator,
    Optional,
    Any,
    IO,
    Callable,
    List,
    Tuple,
    Generic,
    TypeVar,
)
from json import load, dump
from configparser import ConfigParser
from atexit import register
from platform import system, version, architecture, python_version
from copy import deepcopy
from time import perf_counter
from abc import ABC, abstractmethod
from contextlib import contextmanager

from backend.lib.settings import logger, YAML_INSTALLED, TOML_INSTALLED
from backend.lib.settings.exceptions import (
    SanitizationError,
    SaveError,
    LoadError,
    IniFormatError,
)
from backend.lib.settings.utils import (
    set_file_paths,
    set_format,
    composite_toggle,
    filter_locals,
    get_caller_stack,
)


T = TypeVar("T")


class SettingsManagerBase(ABC, Generic[T]):
    def __init__(
        self,
        path: Optional[str] = None,
        autosave: bool = False,
        auto_sanitize: bool = False,
        *,
        config_format: Optional[str] = None,
        default_settings: T,
        read_path: Optional[str] = None,
        write_path: Optional[str] = None,
        autosave_on_exit: bool = False,
        auto_sanitize_on_load: bool = False,
        auto_sanitize_on_save: bool = False,
    ) -> None:

        if not default_settings:
            raise ValueError("default_settings must be provided.")

        self._settings: T
        self._default_settings: T = deepcopy(x=default_settings)

        self._safe_load = None
        self._safe_dump = None
        self._toml_load = None
        self._toml_dump = None

        logger.debug(
            msg=f"\n=========== Initializing SettingsManager ===========\nSystem info: {system()} {version()} {architecture()[0]} Python {python_version()}\n"
        )
        logger.debug(msg=f"args: {filter_locals(locals_dict=locals())}")

        if YAML_INSTALLED:
            logger.debug(msg="YAML module is installed, importing...")
            from yaml import safe_load, safe_dump

            self._safe_load = safe_load
            self._safe_dump = safe_dump
        if TOML_INSTALLED:
            logger.debug(msg="TOML module is installed, importing...")
            from toml import load as toml_load, dump as toml_dump

            self._toml_load = toml_load
            self._toml_dump = toml_dump

        self._read_path, self._write_path = set_file_paths(
            path=path, read_path=read_path, write_path=write_path
        )

        (autosave_on_exit,) = composite_toggle(
            composite=autosave, options=(autosave_on_exit,)
        )

        auto_sanitize_on_load, auto_sanitize_on_save = composite_toggle(
            composite=auto_sanitize,
            options=(auto_sanitize_on_load, auto_sanitize_on_save),
        )

        logger.debug(
            msg=f"Read path: {self._read_path}. Write path: {self._write_path}."
        )

        self._format: str = (
            set_format(config_format=config_format)
            if config_format
            else set_format(read_path=self._read_path, write_path=self._write_path)
        )

        self._auto_sanitize_on_load: bool = auto_sanitize_on_load
        self._auto_sanitize_on_save: bool = auto_sanitize_on_save

        logger.info(msg="Initializing settings data.")
        start: float = perf_counter()
        self._first_time_load()
        end: float = perf_counter()
        logger.info(msg=f"Settings data initialized in {end - start:.6f} seconds.")

        if autosave_on_exit:
            logger.debug(msg="autosave_on_exit is enabled; registering save method.")
            register(self.save)

        logger.debug(
            msg=f"Sanitize settings on: load={self._auto_sanitize_on_load}, save={self._auto_sanitize_on_save}."
        )
        logger.info(msg=f"SettingsManager initialized with format {self._format}!")

    @property
    def settings(self) -> T:
        return self._settings

    @settings.setter
    def settings(self, value: T) -> None:
        self._settings = value

    def _first_time_load(self) -> None:
        """
        Loads the settings from the file if it exists, otherwise applies default settings and saves them to the file.
        """
        if self._read_path.exists():
            logger.info(
                msg=f"Found settings file {self._read_path}; loading settings from file."
            )
            self.load()
        else:
            logger.info(
                msg=f"Could not find settings file {self._read_path}; applying default settings and saving to new file."
            )
            self.settings = deepcopy(x=self._default_settings)
            self.save()  # Save the default settings to the file

    def save(self) -> None:
        """
        Save the settings data to a file.

        If the auto_sanitize flag is set to True, the settings will be sanitized before saving.

        Raises:
            SaveError: If there is an error while writing the settings to the file.
        """
        logger.debug(msg=f"Save requested by {get_caller_stack(instance=self)}...")
        settings_data: Dict[str, Any] = self._to_dict(obj=self.settings)
        if self._auto_sanitize_on_save:
            self.sanitize_settings()
        if self._format == "ini" and not self.valid_ini_format(data=settings_data):
            logger.error(
                msg="The INI format requires top-level keys to be sections, with settings as nested dictionaries. Please ensure your data follows this structure."
            )
            raise IniFormatError(
                "The INI format requires top-level keys to be sections, with settings as nested dictionaries. Please ensure your data follows this structure."
            )
        with open(file=self._write_path, mode="w") as file:
            try:
                self._write(data=settings_data, file=file)
                logger.debug(msg=f"Settings saved to {self._write_path}.")
            except IOError as e:
                logger.exception(msg="Error while writing settings to file.")
                raise SaveError("Error while writing settings to file.") from e

    @contextmanager
    def autosave(self) -> Generator[None, Any, None]:
        """
        A context manager that allows you to save the settings data to a file within a context block.

        If the auto_sanitize flag is set to True, the settings will be sanitized before saving.

        Yields:
            None: The context manager yields None.

        Raises:
            SaveError: If there is an error while writing the settings to the file.
        """
        try:
            yield
        finally:
            self.save()

    def _write(self, data: Dict[str, Any], file: IO) -> None:
        """
        Dispatches the write operation to the correct method based on the format attribute.

        Args:
            data (Dict[str, Any]): The settings data to write to the file.
            file (IO): The file object to write the settings to.
        """
        format_to_function: Dict[str, Callable[[Dict[str, Any], IO], None]] = {
            "json": self._write_as_json,
            "yaml": self._write_as_yaml,
            "toml": self._write_as_toml,
            "ini": self._write_as_ini,
        }
        write_function: Callable[[Dict[str, Any], IO], None] = format_to_function[
            self._format
        ]
        logger.debug(
            msg=f"Format is {self._format}, dispatching write operation to {write_function.__name__}."
        )
        write_function(data, file)

    def _write_as_json(self, data: Dict[str, Any], file: IO) -> None:
        dump(obj=data, fp=file, indent=4)

    def _write_as_yaml(self, data: Dict[str, Any], file: IO) -> None:
        if not self._safe_dump:
            raise ImportError("PyYAML is not installed.")
        self._safe_dump(data, file)

    def _write_as_toml(self, data: Dict[str, Any], file: IO) -> None:
        if not self._toml_dump:
            raise ImportError("TOML is not installed.")
        self._toml_dump(data, file)

    def _write_as_ini(self, data: Dict[str, Any], file: IO) -> None:
        config = ConfigParser(allow_no_value=True)
        for section, settings in data.items():
            config[section] = settings
        config.write(fp=file)

    def load(self) -> None:
        """
        Load the settings from the specified file into the internal data attribute. autosave_on_change is not triggered by this method.

        If the auto_sanitize flag is set to True, the settings will be sanitized after reading.

        Raises:
            LoadError: If there is an error while reading the settings from the file.
        """
        logger.debug(msg=f"Load requested by {get_caller_stack(instance=self)}...")
        with open(file=self._read_path, mode="r") as f:
            try:
                self.settings = self._from_dict(data=self._read(file=f))
                if self._auto_sanitize_on_load:
                    self.sanitize_settings()
                logger.debug(msg=f"Settings loaded from {self._read_path}.")
            except IOError as e:
                logger.exception(msg="Error while reading settings from file.")
                raise LoadError("Error while reading settings from file.") from e

    def _read(self, file: IO) -> Dict[str, Any]:
        """
        Dispatches the read operation to the correct method based on the format attribute.

        Args:
            file (IO): The file object to read the settings from.

        Returns:
            Dict[str, Any]: The settings data read from the file.

        Raises:
            UnsupportedFormatError: If the format is not in the list of supported formats.
        """
        format_to_function: Dict[str, Callable[[IO], Dict[str, Any]]] = {
            "json": self._read_as_json,
            "yaml": self._read_as_yaml,
            "toml": self._read_as_toml,
            "ini": self._read_as_ini,
        }
        read_function: Callable[[IO], Dict[str, Any]] = format_to_function[self._format]
        return read_function(file)

    def _read_as_json(self, file: IO) -> Dict[str, Any]:
        return load(fp=file)

    def _read_as_yaml(self, file: IO) -> Dict[str, Any]:
        if not self._safe_load:
            raise ImportError("PyYAML is not installed.")
        return self._safe_load(file)

    def _read_as_toml(self, file: IO) -> Dict[str, Any]:
        if not self._toml_load:
            raise ImportError("TOML is not installed.")
        return self._toml_load(file)

    def _read_as_ini(self, file: IO) -> Dict[str, Any]:
        config = ConfigParser(allow_no_value=True)
        config.read_file(f=file)
        return {
            section: dict(config.items(section=section))
            for section in config.sections()
        }

    def sanitize_settings(self) -> None:
        """
        Sanitizes the settings data by applying the default settings and removing any invalid or unnecessary values.

        The sanitization process is directly applied to the internal data attribute.

        Raises:
            SanitizationError: If an error occurs while sanitizing the settings.

        """
        logger.debug(
            msg=f"Sanitization requested by {get_caller_stack(instance=self)}..."
        )
        settings: Dict[str, Any] = self._to_dict(obj=self.settings)
        default_settings: Dict[str, Any] = self._to_dict(obj=self._default_settings)

        try:
            keys_to_remove, keys_to_add = self._sanitize_settings(
                settings=settings,
                default_settings=default_settings,
                dict_path="",
            )

            logger.debug(msg=f"Got {len(keys_to_remove)} total keys to remove.")
            if keys_to_remove:
                logger.debug(msg=f"{keys_to_remove}")
            logger.debug(msg=f"Got {len(keys_to_add)} total keys to add.")
            if keys_to_add:
                logger.debug(msg=f"{keys_to_add}")

            for key in keys_to_remove:
                logger.debug(msg=f"Removing key: {key}")
                self._remove_key(key=key)

            for key, value in keys_to_add.items():
                logger.debug(msg=f"Adding key: {key} with value: {value}")
                self._add_key(key=key, value=value)
        except SanitizationError as e:
            logger.exception(msg="Error while sanitizing settings.")
            raise e

    def _sanitize_settings(
        self,
        settings: Dict[str, Any],
        default_settings: Dict[str, Any],
        dict_path: str,
    ) -> Tuple[List[str], Dict[str, Any]]:

        keys_to_remove: List[str] = []
        keys_to_add: Dict[str, Any] = {}

        logger.debug(
            msg=f"Checking settings in dict_path: {dict_path if dict_path else 'root'}..."
        )

        for key in settings:
            current_path: str = f"{dict_path}.{key}" if dict_path else key
            if key not in default_settings:
                keys_to_remove.append(current_path)
                logger.debug(
                    msg=f"Found and added key {current_path} to key removal list."
                )
            elif isinstance(settings[key], dict):
                logger.debug(msg=f"{current_path} is a dictionary; Recursing...")
                nested_keys_to_remove, nested_keys_to_add = self._sanitize_settings(
                    settings=settings[key],
                    default_settings=default_settings[key],
                    dict_path=current_path,
                )
                keys_to_remove.extend(nested_keys_to_remove)
                keys_to_add.update(nested_keys_to_add)
                logger.debug(
                    msg=f"Finished recursion for {current_path}. Got {len(nested_keys_to_remove)} keys to remove and {len(nested_keys_to_add)} keys to add."
                )

        for key in default_settings:
            if key not in settings:
                keys_to_add[f"{dict_path}.{key}" if dict_path else key] = (
                    default_settings[key]
                )
                logger.debug(msg=f"Added missing key {key} to key addition list.")

        return keys_to_remove, keys_to_add

    def _remove_key(self, key: str) -> None:
        """
        Removes the key from the settings data.

        Args:
            key (str): The key to remove from the settings data.
        """
        keys: List[str] = key.split(sep=".")
        settings: Dict[str, Any] = self._to_dict(obj=self.settings)
        current_dict: Dict[str, Any] = settings

        # Traverse the settings data to the parent of the key to remove
        for key in keys[:-1]:
            current_dict = current_dict[key]

        del current_dict[keys[-1]]

        # Maybe it's the lack of sleep and caffeine, but in case you are as confused as I initially was on how this works:
        # 'settings' is a dictionary that represents the settings data, and 'current_dict' is initially a reference to the same dictionary.
        # As we traverse the settings data, we aren't modifying the entire 'settings' dictionary directly,
        # but rather we change the 'current_dict' reference to point to the nested dictionary (the parent of the key to remove).
        # Instead of 'current_dict' referencing the whole 'settings' dictionary, it now references the nested dictionary that contains the key to remove.
        # Since 'current_dict' and the nested dictionary in 'settings' are still the same object in memory,
        # deleting the key from 'current_dict' also deletes it from the corresponding dictionary inside 'settings'.

        # Update the settings data with the modified dictionary
        self.settings = self._from_dict(data=settings)

    def _add_key(self, key: str, value: Any) -> None:
        """
        Adds the key with the specified value to the settings data.

        Args:
            key (str): The key to add to the settings data.
            value (Any): The value to associate with the key.
        """
        keys: List[str] = key.split(sep=".")
        settings: Dict[str, Any] = self._to_dict(obj=self.settings)
        current_dict: Dict[str, Any] = settings

        # Traverse the settings data to the parent of the key to add
        for key in keys[:-1]:
            current_dict = current_dict[key]
        current_dict[keys[-1]] = value

        # Update the settings data with the modified dictionary
        self.settings = self._from_dict(data=settings)

    @staticmethod
    def valid_ini_format(data: Dict[str, Any]) -> bool:
        """
        Checks if all top-level keys have nested dictionaries as values.

        Args:
            data (Dict[str, Any]): The settings data to check.

        Returns:
            bool: True if all top-level keys have nested dictionaries as values, False otherwise.
        """
        for _, settings in data.items():
            if not isinstance(settings, dict):
                return False
        return True

    @abstractmethod
    def _to_dict(self, obj: Any) -> Dict[str, Any]:
        """
        Converts the settings object to a dictionary.

        Various internal methods require the settings data to be in dictionary format, so this method is used to convert the settings object to a dictionary.
        It is the subclass's responsibility to implement this method to correctly handle and convert the settings object to a dictionary.
        """
        ...

    @abstractmethod
    def _from_dict(self, data: Dict[str, Any]) -> T:
        """
        Converts the dictionary data to a settings object.

        Any internal method that has worked on the dictionary representation of the settings data will need to convert it back to the settings object.
        It is the subclass's responsibility to implement this method to correctly handle and convert the dictionary data to the settings object.
        """
        ...
