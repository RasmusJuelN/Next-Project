from pydantic import BaseModel, AliasGenerator, ConfigDict
from pydantic.alias_generators import to_camel
from typing import Literal


class CamelCaseModel(BaseModel):
    model_config = ConfigDict(
        alias_generator=AliasGenerator(alias=to_camel, validation_alias=to_camel),
        populate_by_name=True,
    )


class LogEntry(CamelCaseModel):
    """
    A Pydantic model representing a log entry.
    """

    timestamp: str
    severity: Literal["DEBUG", "INFO", "WARNING", "ERROR", "CRITICAL"]
    source: str
    message: str
