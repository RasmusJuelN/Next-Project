from pydantic import BaseModel, ConfigDict, AliasGenerator
from pydantic.alias_generators import to_camel

from backend.lib.settings.models import AppSettings, AppSettingsMetadata


class CamelCaseModel(BaseModel):
    model_config = ConfigDict(
        alias_generator=AliasGenerator(alias=to_camel, validation_alias=to_camel),
        populate_by_name=True,
    )


class SettingsWithMetadata(CamelCaseModel):
    settings: AppSettings
    metadata: AppSettingsMetadata = AppSettingsMetadata()
