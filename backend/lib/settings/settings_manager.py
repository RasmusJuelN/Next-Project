from __future__ import annotations
from json import dumps, loads
from dacite import from_dict
from dataclasses import asdict
from typing import Any, Dict, TypeVar, TYPE_CHECKING, Union


from backend.lib.settings.base import SettingsManagerBase

if TYPE_CHECKING:
    from _typeshed import DataclassInstance

T = TypeVar("T")


class TemplateSettings:
    """
    Used by SettingsManagerClass to convert a dictionary to an object using json.loads and json.dumps.
    """

    def __init__(self, dict: dict) -> None:
        self.__dict__.update(dict)


class SettingsManagerWithDataclass(SettingsManagerBase[T]):
    def _to_dict(self, obj: "DataclassInstance") -> Dict[str, Any]:
        """
        Converts the settings object to a dictionary using dataclasses.asdict.

        Args:
            obj (object): The settings object to convert to a dictionary.

        Returns:
            Dict[str, Any]: The settings object converted to a dictionary.
        """
        return asdict(obj)

    def _from_dict(self, data: Dict[str, Any]) -> T:
        """
        Converts the dictionary data to a settings object using dacite.from_dict.

        Args:
            data (Dict[str, Any]): The dictionary data to convert to a settings object.

        Returns:
            T: The dictionary data converted to a settings object.
        """
        return from_dict(data_class=self._default_settings.__class__, data=data)


class SettingsManagerWithClass(SettingsManagerBase[T]):
    def _to_dict(self, obj: object) -> Dict[str, Any]:
        """
        Converts the settings object to a dictionary a custom method which iterates through the object's attributes.

        Args:
            obj (object): _description_

        Returns:
            Dict[str, Any]: _description_
        """
        new_dict = self._class_to_dict(obj=obj)
        if not isinstance(new_dict, dict):
            raise TypeError("Settings object must be a dictionary.")
        return new_dict

    def _from_dict(self, data: Dict[str, Any]) -> T:
        """
        Converts the dictionary data to a settings object using json.loads and json.dumps.

        Args:
            data (Dict[str, Any]): The dictionary data to convert to a settings object.

        Returns:
            T: The dictionary data converted to a settings object.
        """
        return loads(s=dumps(obj=data), object_hook=TemplateSettings)

    def _class_to_dict(self, obj: object) -> Union[dict, list, Dict[str, Any], object]:
        """
        Recursively converts a given object to a dictionary representation.

        Args:
            obj (object): The object to be converted.

        Returns:
            dict | list | dict[str, Any] | object: The dictionary representation of the object.

        """
        if isinstance(obj, dict):
            return {key: self._class_to_dict(obj=obj) for key, obj in obj.items()}
        elif isinstance(obj, list):
            return [self._class_to_dict(obj=obj) for obj in obj]
        elif hasattr(obj, "__dict__"):
            return {
                key: self._class_to_dict(obj=value) for key, value in vars(obj).items()
            }
        else:
            return obj
