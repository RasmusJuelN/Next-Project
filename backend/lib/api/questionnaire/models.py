from pydantic import BaseModel, AliasGenerator, ConfigDict, Field
from pydantic.alias_generators import to_camel
from typing import Optional


class CamelCaseModel(BaseModel):
    model_config = ConfigDict(
        alias_generator=AliasGenerator(alias=to_camel, validation_alias=to_camel),
        populate_by_name=True,
    )


class TemplateSearchRequest(CamelCaseModel):
    page: int = Field(default=1, ge=1, description="The page number for pagination")
    limit: int = Field(
        default=10, le=10, ge=1, description="The number of results per page"
    )
    title: Optional[str] = Field(
        default=None, description="The title of the template to search for"
    )


class QuestionnaireSearchRequest(CamelCaseModel):
    page: int = Field(default=1, ge=1, description="The page number for pagination")
    limit: int = Field(
        default=10, le=10, ge=1, description="The number of results per page"
    )
    search_student: str = Field(
        default="%",
        description="The student ID to search for. Defaults to '%', which means all students",
    )
    search_teacher: str = Field(
        default="%",
        description="The teacher ID to search for. Defaults to '%', which means all teachers",
    )
