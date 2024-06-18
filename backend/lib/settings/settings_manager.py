"""
Settings Manager module for handling settings files in JSON, YAML, TOML, and INI formats.

Author: Nicklas H. (LobaDK)
Date: 2024

This module provides a SettingsManager convenience class for handling settings and configuration files in JSON, YAML, TOML, and INI formats. It is provided "as is" for anyone to use, modify, and distribute, freely and openly. While not required, credit back to the original author is appreciated.

This module is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
"""

from logging import Logger
from typing import (
    Dict,
    Optional,
    Any,
    Iterator,
    TypeVar,
    Type,
    Union,
)
from pathlib import Path
from json import load, dump
from configparser import ConfigParser
from collections.abc import MutableMapping
from atexit import register
from dacite import from_dict
from dataclasses import asdict, is_dataclass

T = TypeVar("T")

# Initialize flags indicating the availability of optional modules
yaml_available = False
toml_available = False
logging_available = False

# Attempt to import optional modules and set flags accordingly
try:
    from yaml import safe_load, safe_dump

    yaml_available = True
except ImportError:
    pass

try:
    from toml import load as toml_load, dump as toml_dump

    toml_available = True
except ImportError:
    pass

try:
    from .._logger import LogHelper

    logging_available = True
except ImportError:
    pass

SUPPORTED_FORMATS: list[str] = ["json", "yaml", "toml", "ini"]


class SettingsManager(MutableMapping):
    """
    A class for managing application settings, supporting various file formats and providing functionality for reading, writing, and sanitizing settings.

    This class acts as a mutable mapping, allowing settings to be accessed and modified like a dictionary. It supports reading from and writing to files in JSON, YAML, TOML, and INI formats, with automatic format detection based on file extension. The class also offers features like saving settings automatically on changes or on application exit, sanitizing settings to match a provided schema of default settings, and optional logging of operations.

    Attributes:
        data (Dict): The settings data stored in the SettingsManager instance. Although accessible, it is recommended to interact with the settings using the dictionary-like interface provided by the class.

    Raises:
        ValueError: If both `path` and either `read_path` or `write_path` are provided, or if none are provided.
        ValueError: If `default_settings` is not provided.
        ValueError: If the specified or detected format is not supported or if required modules for the format are not available.
        TypeError: If default_settings is neither a dictionary nor a dataclass instance.

    Examples:
        Initializing `SettingsManager` with a JSON file:

        ```python
        settings_manager = SettingsManager(
            path="settings.json",
            default_settings={"theme": "light", "notifications": True},
            save_on_change=True
        )
        ```

        Initializing `SettingsManager` with a JSON file and a dataclass instance:
        ```python
        @dataclass
        class Settings:
            theme: str = "light"
            notifications: bool = True

        default_settings = Settings()
        settings_manager = SettingsManager(
            path="settings.json",
            default_settings=default_settings,
            save_on_change=True
        )
        ```

        Reading and updating a setting:

        ```python
        current_theme = settings_manager["theme"]
        settings_manager["theme"] = "dark"
        ```

        Reading and updating a setting using a dataclass instance:

        ```python
        settings = settings_manager.to_object(Settings)
        current_theme = settings.theme
        settings.theme = "dark"
        ```

        Saving settings manually:

        ```python
        settings_manager.save()
        ```

    Note:
        There is currently no way to save settings set in a dataclass instance back to the settings file. You can convert the settings to a dictionary using the `asdict` function from the `dataclasses` module and set the `data` attribute of the `SettingsManager` instance to the dictionary before saving.
    """

    def __init__(
        self,
        path: Optional[str] = None,
        /,
        *,
        read_path: Optional[str] = None,
        write_path: Optional[str] = None,
        default_settings: Union[Dict[str, Any], object],
        save_on_exit: bool = False,
        save_on_change: bool = False,
        use_logger: bool = False,
        log_file: Optional[str] = None,
        sanitize: bool = False,
        format: Optional[str] = None,
    ) -> None:
        """
        Initialize the SettingsManager object.

        Args:
            path (Optional[str], optional): The path and name of the settings file being written to and read from. Defaults to None.
            read_path (Optional[str], optional): The path and name of the settings file being read from. Defaults to None.
            write_path (Optional[str], optional): The path and name of settings file being written to. Defaults to None.
            default_settings (Union[Dict[str, Any], object]): The default settings to use when the settings file does not exist or is missing keys. Can be a dictionary or a dataclass instance.
            save_on_exit (bool, optional): Whether to save the settings when the program exits. Defaults to False.
            save_on_change (bool, optional): Whether to save the settings when they are changed. May cause slowdowns if the settings are changed frequently. Try and update the settings in bulk. Defaults to False.
            use_logger (bool, optional): Whether to use a Logger. If false, only severe errors will be printed using `print()`. Defaults to False.
            log_file (str, optional): The path and name of the log file. Defaults to None.
            sanitize (bool, optional): Whether to sanitize and check the settings before reading/writing. Defaults to False.
            format (Optional[str], optional): The format is automatically guessed from the extension, but this can be used to override it. Defaults to None.

        Raises:
            ValueError: If both `path` and either `read_path` or `write_path` are provided, or if none are provided.
            ValueError: If `default_settings` is not provided.
            ValueError: If the specified or detected format is not supported or if required modules for the format are not available.
        """
        if not path and not (read_path or write_path):
            raise ValueError("You must provide a path or read_path and write_path.")
        if path and (read_path or write_path):
            raise ValueError(
                "You must provide a path or read_path and write_path, not both."
            )
        if not default_settings:
            raise ValueError("You must provide default_settings.")
        if not logging_available and use_logger:
            raise ValueError("The log_helper module is not available.")
        if not use_logger and log_file:
            raise ValueError("You must enable use_logger to use a log file.")

        if use_logger:
            self.logger: Logger = LogHelper.create_logger(
                logger_name="SettingsManager",
                log_file="settings.log" if not log_file else log_file,
            )

        if path:
            self._read_path: str = path
            self._write_path: str = path
        elif read_path and write_path:
            self._read_path = read_path
            self._write_path = write_path

        self._default_settings: Union[Dict[str, Any], object] = default_settings
        self._save_on_exit: bool = save_on_exit
        self._save_on_change: bool = save_on_change
        self._use_logger: bool = use_logger
        self._sanitize: bool = sanitize

        self._data: Dict[Any, Any] = {}

        if format:
            self._format: str = format
        else:
            self._format = self._get_format()

        if self._format not in SUPPORTED_FORMATS:
            self._log_or_print(
                message=f"User tried to use unsupported format {self._format}."
            )
            raise ValueError(
                f"Format {self._format} is not in the list of supported formats: {', '.join(SUPPORTED_FORMATS)}."
            )

        if self._format == "yaml" and not yaml_available:
            self._log_or_print(
                message="User tried to use yaml format without the yaml module."
            )
            raise ValueError("The yaml module is not available.")
        elif self._format == "toml" and not toml_available:
            self._log_or_print(
                message="User tried to use toml format without the toml module."
            )
            raise ValueError("The toml module is not available.")

        if save_on_exit:
            self._log_or_print(
                message="save_on_exit is enabled; registering save method."
            )
            register(self.save)

        if Path(self._read_path).exists():
            self._log_or_print(
                message=f"Settings file {self._read_path} exists; loading settings."
            )
            self.load()
        else:
            self._log_or_print(
                message=f"Settings file {self._read_path} does not exist; applying default settings and saving."
            )
            self.load_from_default()
            self.save()

        self._log_or_print(
            message=f"Is save_on_change enabled? {self._save_on_change}."
        )
        self._log_or_print(
            message=f"SettingsManager initialized with format {self._format}."
        )

    @property
    def data(self) -> Dict:
        return self._data

    @data.setter
    def data(self, value: Dict) -> None:
        """
        Set the settings data and optionally save it.

        Args:
            value (Dict): The settings data to set.
        """
        self._data = value
        if self._save_on_change:
            self.save()

    def _get_format(self) -> str:
        """
        Determines the format of the file based on its extension.

        Returns:
            str: The format of the file (json, yaml, toml, ini).

        Raises:
            ValueError: If the file extension is not supported.
        """
        if self._read_path.endswith(".json"):
            return "json"
        elif self._read_path.endswith(".yaml") or self._read_path.endswith(".yml"):
            return "yaml"
        elif self._read_path.endswith(".toml"):
            return "toml"
        elif self._read_path.endswith(".ini"):
            return "ini"
        else:
            raise ValueError(
                f"Trying to determine format from file extension, got {self._read_path} but only {', '.join(SUPPORTED_FORMATS)} are supported."
            )

    def load(self) -> None:
        """
        Load the settings from a file.

        The format of the file is determined by the `format` attribute of the `SettingsManager` instance.

        Raises:
            IOError: If there is an error while reading the settings from the file.

        """
        if self._format == "json":
            with open(file=self._read_path, mode="r") as f:
                self.data = load(fp=f)
        elif self._format == "yaml":
            with open(file=self._read_path, mode="r") as f:
                self.data = safe_load(f)
        elif self._format == "toml":
            with open(file=self._read_path, mode="r") as f:
                self.data = toml_load(f)
        elif self._format == "ini":
            config = ConfigParser(allow_no_value=True)
            config.read(filenames=self._read_path)
            self.data = {
                section: dict(config.items(section=section))
                for section in config.sections()
            }
        if self._sanitize:
            self.sanitize_settings()

    def save(self) -> None:
        """
        Save the settings to a file.

        If the `sanitize` flag is set to True, the settings will be sanitized before saving.
        The format of the file is determined by the `format` attribute of the `SettingsManager` instance.

        Raises:
            IOError: If there is an error while writing the settings to the file.

        """
        if self._sanitize:
            self.sanitize_settings()
        if self._format == "json":
            with open(file=self._write_path, mode="w") as file:
                dump(obj=self.data, fp=file, indent=4)
        elif self._format == "yaml" and yaml_available:
            with open(file=self._write_path, mode="w") as file:
                safe_dump(self.data, file)
        elif self._format == "toml" and toml_available:
            with open(file=self._write_path, mode="w") as file:
                toml_dump(self.data, file)
        elif self._format == "ini":
            config = ConfigParser(allow_no_value=True)
            for section, settings in self.data.items():
                config[section] = settings
            with open(file=self._write_path, mode="w") as file:
                config.write(fp=file)

    def sanitize_settings(self) -> None:
        """
        Sanitizes the settings data by removing any keys that are not present in the default settings
        and adding any missing keys from the default settings with their default values.
        """
        _default_settings: Dict
        if isinstance(self._default_settings, dict):
            _default_settings = self._default_settings
        elif is_dataclass(obj=self._default_settings):
            try:
                _default_settings = asdict(obj=self._default_settings)  # type: ignore
            except TypeError as e:
                raise TypeError(
                    f"default_settings must be a dict or a dataclass instance, not {type(self._default_settings)}."
                ) from e
        keys_to_remove = [key for key in self.data if key not in _default_settings]
        for key in keys_to_remove:
            del self.data[key]

        for key, value in _default_settings.items():
            if key not in self.data:
                self.data[key] = value

    def _log_or_print(self, message: str, level: str = "info") -> None:
        if self._use_logger:
            if level == "info":
                self.logger.info(msg=message)
            elif level == "warning":
                self.logger.warning(msg=message)
            elif level == "error":
                self.logger.error(msg=message)
            elif level == "critical":
                self.logger.critical(msg=message)
            elif level == "exception":
                self.logger.exception(msg=message)
        elif level == "error" or level == "critical":
            print(f"{level.upper()} ({__name__}): {message}")

    def to_object(self, data_class: Type[T]) -> T:
        """
        Converts the data stored in the settings manager to an object of the specified data class.

        Args:
            data_class (Type[T]): The class of the object to convert the data to.

        Returns:
            T: An object of the specified data class with the converted data.
        """
        return from_dict(data_class=data_class, data=self.data)

    def load_from_default(self) -> None:
        """
        Loads the default settings into the data attribute.

        If the default_settings attribute is a dictionary, it directly assigns it to the data attribute.
        If the default_settings attribute is a dataclass instance, it converts it to a dictionary using the asdict function from the dataclasses module.

        Raises:
            TypeError: If default_settings is neither a dictionary nor a dataclass instance.
        """
        if isinstance(self._default_settings, dict):
            self.data = self._default_settings
        elif is_dataclass(obj=self._default_settings):
            try:
                self.data = asdict(obj=self._default_settings)  # type: ignore
            except TypeError as e:
                raise TypeError(
                    f"default_settings must be a dict or a dataclass instance, not {type(self._default_settings)}."
                ) from e
        else:
            raise TypeError(
                f"default_settings must be a dict or a dataclass instance, not {type(self._default_settings)}."
            )

    def __getitem__(self, key) -> Any:
        return self.data[key]

    def __setitem__(self, key, value) -> None:
        self.data[key] = value

    def __delitem__(self, key) -> None:
        del self.data[key]

    def __iter__(self) -> Iterator:
        return iter(self.data)

    def __len__(self):
        return len(self.data)
