from fastapi import Request, Response
from starlette.middleware.base import BaseHTTPMiddleware, RequestResponseEndpoint
from importlib import import_module
from types import ModuleType
from typing import Any, Dict


class I18nMiddleware(BaseHTTPMiddleware):
    LAN_LIST: list[str] = ["en-US", "en-GB", "da-DK"]

    async def dispatch(
        self, request: Request, call_next: RequestResponseEndpoint
    ) -> Response:
        language = request.headers.get("Accept-Language", default="en")

        # Accept-Language header example: "en-US,en-GB;q=0.9,en;q=0.8"
        for lang in self.LAN_LIST:
            if lang in language:
                request.state.lang = lang
                break
        else:
            request.state.lang = "en-US"

        response: Response = await call_next(request)
        return response


class Translator:
    _instances: Dict[str, "Translator"] = {}

    def __new__(cls, lang: str) -> "Translator":
        if lang not in cls._instances:
            cls._instances[lang] = super(Translator, cls).__new__(cls)
        return cls._instances[lang]

    def __init__(self, lang: str) -> None:
        self.lang: str = lang

    def t(self, key: str, **kwargs: Dict[str, Any]) -> str:
        file_key, *translation_keys = key.split(sep=".")

        locale_module: ModuleType = import_module(
            name=f"lib.lang.{self.lang}.{file_key}"
        )

        translations: Dict[str, str] = locale_module.locale
        for translation_key in translation_keys:
            translation: str | None = translations.get(translation_key, None)
        if kwargs.keys() and translation is not None:
            translation = translation.format(**kwargs)
        if translation is not None:
            return translation
        return f"Key {key} not found in {self.lang} locale"
