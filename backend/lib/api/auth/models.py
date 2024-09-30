from pydantic import BaseModel, ConfigDict, AliasGenerator
from pydantic.alias_generators import to_camel
from typing import Optional


class CamelBaseModel(BaseModel):
    model_config = ConfigDict(
        alias_generator=AliasGenerator(
            validation_alias=to_camel,
            serialization_alias=to_camel,
        ),
        populate_by_name=True,
    )


class TokenData(CamelBaseModel):
    username: Optional[str] = None
    full_name: Optional[str] = None
    scope: Optional[str] = None
    uuid: Optional[str] = None


class User(CamelBaseModel):
    user_name: str
    full_name: str
    role: str
