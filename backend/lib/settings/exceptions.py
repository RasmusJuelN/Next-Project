class SettingsException(Exception):
    """Base exception class for settings-related errors."""

    pass


class InvalidPathError(SettingsException):
    """Exception raised when an invalid path is provided."""

    pass


class MissingPathError(InvalidPathError):
    """Exception raised when a path is missing."""

    pass


class TooManyPathsError(InvalidPathError):
    """Exception raised when too many paths are provided."""

    pass


class UnsupportedFormatError(SettingsException):
    """Exception raised when an unsupported settings file format is specified."""

    pass


class MissingDependencyError(SettingsException):
    """Exception raised when a required dependency for a settings format is missing."""

    pass


class TOMLNotInstalledError(MissingDependencyError):
    """Exception raised when the TOML module is not installed."""

    pass


class YAMLNotInstalledError(MissingDependencyError):
    """Exception raised when the PyYAML module is not installed."""

    pass


class SanitizationError(SettingsException):
    """Exception raised during sanitization of settings data."""

    pass


class SaveError(SettingsException):
    """Exception raised when saving settings fails."""

    pass


class LoadError(SettingsException):
    """Exception raised when loading settings fails."""

    pass


class IniFormatError(SettingsException):
    """Exception raised when an ini file is not formatted correctly."""

    pass
