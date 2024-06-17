"""
Settings Manager module for handling settings files in JSON, YAML, TOML, and INI formats.

Author: Nicklas H. (LobaDK)
Date: 2024

This module provides a SettingsManager convenience class for handling settings and configuration files in JSON, YAML, TOML, and INI formats. It is provided "as is" for anyone to use, modify, and distribute, freely and openly. While not required, credit back to the original author is appreciated.

This module is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
"""

from logging import Logger
from typing import Dict, Optional, Any
from pathlib import Path
from json import load, dump
from configparser import ConfigParser
from collections.abc import MutableMapping
from atexit import register


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
        read_path (str): Path to the settings file to read from. If `path` is provided, this is the same as `write_path`.
        write_path (str): Path to the settings file to write to. If `path` is provided, this is the same as `read_path`.
        default_settings (Dict): A dictionary of default settings.
        save_on_exit (bool): Whether to save settings automatically when the application exits.
        save_on_change (bool): Whether to save settings automatically whenever they are changed.
        use_logger (bool): Whether to log operations using a logger.
        sanitize (bool): Whether to sanitize settings to ensure they match the provided default settings.
        format (str): The format of the settings file. Detected automatically from the file extension or can be set manually.

    Raises:
        ValueError: If both `path` and either `read_path` or `write_path` are provided, or if none are provided.
        ValueError: If `default_settings` is not provided.
        ValueError: If the specified or detected format is not supported or if required modules for the format are not available.

    Examples:
        Initializing `SettingsManager` with a JSON file:

        ```python
        settings_manager = SettingsManager(
            path="settings.json",
            default_settings={"theme": "light", "notifications": True},
            save_on_change=True
        )
        ```

        Reading and updating a setting:

        ```python
        current_theme = settings_manager["theme"]
        settings_manager["theme"] = "dark"
        ```

        Saving settings manually:

        ```python
        settings_manager.save()
        ```
    """

    def __init__(
        self,
        path: Optional[str] = None,
        /,
        *,
        read_path: Optional[str] = None,
        write_path: Optional[str] = None,
        default_settings: Dict,
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
            default_settings (Union[Dict, object]): An object of attributes or a dictionary of default settings. These settings will be used if the settings file does not exist, replaces missing settings or reset the settings.
            save_on_exit (bool, optional): Whether to save the settings when the program exits. Defaults to False.
            save_on_change (bool, optional): Whether to save the settings when they are changed. May cause slowdowns if the settings are changed frequently. Try and update the settings in bulk. Defaults to False.
            use_logger (bool, optional): Whether to use a Logger. If false, only severe errors will be printed using `print()`. Defaults to False.
            log_file (str, optional): The path and name of the log file. Defaults to None.
            sanitize (bool, optional): Whether to sanitize and check the settings before reading/writing. Defaults to False.
            format (Optional[str], optional): The format is automatically guessed from the extension, but this can be used to override it. Defaults to None.

        Raises:
            ValueError: If path and read_path/write_path are both provided.
            ValueError: If settings and default_settings are not provided.
            ValueError: If the file format is not supported.
            ValueError: If the format detected or specified requires a module that is not available.
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
            self.read_path: str = path
            self.write_path: str = path
        elif read_path and write_path:
            self.read_path = read_path
            self.write_path = write_path

        self.default_settings: Dict = default_settings
        self.save_on_exit: bool = save_on_exit
        self.save_on_change: bool = save_on_change
        self.use_logger: bool = use_logger
        self.sanitize: bool = sanitize

        self._data: Dict[Any, Any] = {}

        if format:
            self.format: str = format
        else:
            self.format = self._get_format()

        if self.format not in SUPPORTED_FORMATS:
            self.log_or_print(
                message=f"User tried to use unsupported format {self.format}."
            )
            raise ValueError(
                f"Format {self.format} is not in the list of supported formats: {', '.join(SUPPORTED_FORMATS)}."
            )

        if self.format == "yaml" and not yaml_available:
            self.log_or_print(
                message="User tried to use yaml format without the yaml module."
            )
            raise ValueError("The yaml module is not available.")
        elif self.format == "toml" and not toml_available:
            self.log_or_print(
                message="User tried to use toml format without the toml module."
            )
            raise ValueError("The toml module is not available.")

        if save_on_exit:
            self.log_or_print(
                message="save_on_exit is enabled; registering save method."
            )
            register(self.save)

        if Path(self.read_path).exists():
            self.log_or_print(
                message=f"Settings file {self.read_path} exists; loading settings."
            )
            self._load()
        else:
            self.log_or_print(
                message=f"Settings file {self.read_path} does not exist; using default settings."
            )
            self.data = self.default_settings
        self.log_or_print(message=f"Is save_on_change enabled? {self.save_on_change}.")
        self.log_or_print(
            message=f"SettingsManager initialized with format {self.format}."
        )

    @property
    def data(self) -> Dict:
        return self._data

    @data.setter
    def data(self, value: Dict) -> None:
        self._data = value
        if self.save_on_change:
            self.save()

    def _get_format(self) -> str:
        if self.read_path.endswith(".json"):
            return "json"
        elif self.read_path.endswith(".yaml") or self.read_path.endswith(".yml"):
            return "yaml"
        elif self.read_path.endswith(".toml"):
            return "toml"
        elif self.read_path.endswith(".ini"):
            return "ini"
        else:
            raise ValueError(
                f"Trying to determine format from file extension, got {self.read_path} but only {', '.join(SUPPORTED_FORMATS)} are supported."
            )

    def _load(self):
        if self.format == "json":
            with open(self.read_path, "r") as f:
                self.data = load(f)
        elif self.format == "yaml":
            with open(self.read_path, "r") as f:
                self.data = safe_load(f)
        elif self.format == "toml":
            with open(self.read_path, "r") as f:
                self.data = toml_load(f)
        elif self.format == "ini":
            config = ConfigParser(allow_no_value=True)
            config.read(self.read_path)
            self.data = {
                section: dict(config.items(section)) for section in config.sections()
            }
        if self.sanitize:
            self.sanitize_settings()

    def save(self) -> None:
        if self.sanitize:
            self.sanitize_settings()
        if self.format == "json":
            with open(self.write_path, "w") as file:
                dump(self.data, file, indent=4)
        elif self.format == "yaml" and yaml_available:
            with open(self.write_path, "w") as file:
                safe_dump(self.data, file)
        elif self.format == "toml" and toml_available:
            with open(self.write_path, "w") as file:
                toml_dump(self.data, file)
        elif self.format == "ini":
            config = ConfigParser(allow_no_value=True)
            for section, settings in self.data.items():
                config[section] = settings
            with open(self.write_path, "w") as file:
                config.write(file)

    def sanitize_settings(self) -> None:
        keys_to_remove = [key for key in self.data if key not in self.default_settings]
        for key in keys_to_remove:
            del self.data[key]

        for key, value in self.default_settings.items():
            if key not in self.data:
                self.data[key] = value

    def log_or_print(self, message: str, level: str = "info") -> None:
        if self.use_logger:
            if level == "info":
                self.logger.info(message)
            elif level == "warning":
                self.logger.warning(message)
            elif level == "error":
                self.logger.error(message)
            elif level == "critical":
                self.logger.critical(message)
            elif level == "exception":
                self.logger.exception(message)
        elif level == "error" or level == "critical":
            print(f"{level.upper()} ({__name__}): {message}")

    def __getitem__(self, key):
        return self.data[key]

    def __setitem__(self, key, value):
        self.data[key] = value

    def __delitem__(self, key):
        del self.data[key]

    def __iter__(self):
        return iter(self.data)

    def __len__(self):
        return len(self.data)
