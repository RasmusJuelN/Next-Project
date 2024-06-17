from fastapi import Request, Response
from starlette.middleware.base import BaseHTTPMiddleware, RequestResponseEndpoint
from importlib import import_module
from types import ModuleType
from typing import Any, Dict


class I18nMiddleware(BaseHTTPMiddleware):
    """
    Middleware class for handling internationalization (i18n) in HTTP requests.

    This middleware extracts the language from the "Accept-Language" header in the request
    and sets it in the request state. If the language is not found in the accepted language list,
    the default language is set to "en-US".

    Attributes:
        LAN_LIST (list[str]): List of accepted languages.

    Methods:
        dispatch: Middleware method that sets the language in the request state and calls the next middleware.
    """

    LAN_LIST: list[str] = ["en-US", "en-GB", "da-DK"]

    async def dispatch(
        self, request: Request, call_next: RequestResponseEndpoint
    ) -> Response:
        """
        Middleware method that sets the language in the request state and calls the next middleware.

        Args:
            request (Request): The incoming HTTP request.
            call_next (RequestResponseEndpoint): The next middleware or endpoint to call.

        Returns:
            Response: The HTTP response returned by the next middleware or endpoint.
        """
        languages: str = request.headers.get("Accept-Language", default="en-US")

        # Accept-Language header example: "en-US,en-GB;q=0.9,en;q=0.8"
        for language in languages.split(sep=","):
            lang: str = language.split(sep=";")[0]
            if lang in self.LAN_LIST:
                request.state.language = lang
                break
        else:
            request.state.language = "en-US"

        response: Response = await call_next(request)
        return response


class Translator:
    """
    Singleton class for translating strings based on language-specific translation files.

    Args:
        lang (str): The language code for the translator instance.

    Attributes:
        lang (str): The language code for the translator instance.

    Methods:
        t(key: str, **kwargs: Dict[str, Any]) -> str:
            Translates the given key using the language-specific translation files.

    """

    _instances: Dict[str, "Translator"] = {}

    def __new__(cls, lang: str) -> "Translator":
        """
        Create a new instance of the Translator class if it doesn't already exist for the specified language.

        Args:
            lang (str): The language for which to create the Translator instance.

        Returns:
            Translator: The existing or newly created Translator instance for the specified language.
        """
        if lang not in cls._instances:
            cls._instances[lang] = super(Translator, cls).__new__(cls)
        return cls._instances[lang]

    def __init__(self, lang: str) -> None:
        self.lang: str = lang

    def t(self, key: str, **kwargs: Dict[str, Any]) -> str:
        """
        Translates the given key to the corresponding string in the specified language.

        Args:
            key (str): The translation key.
            **kwargs (Dict[str, Any]): Optional keyword arguments to be used for string formatting.

        Returns:
            str: The translated string.

        Raises:
            KeyError: If the translation key is not found in the specified language.

        """
        file_key, *translation_keys = key.split(sep=".")

        locale_module: ModuleType = import_module(
            name=f"backend.lib.lang.{self.lang}.{file_key}"
        )

        translations: Dict[str, str] = locale_module.locale
        for translation_key in translation_keys:
            translation: str | None = translations.get(translation_key, None)
        if kwargs.keys() and translation is not None:
            translation = translation.format(**kwargs)
        if translation is not None:
            return translation
        raise KeyError(f"Key {key} not found in {self.lang} locale")
