from datetime import datetime
from typing import List, Optional
from pydantic import BaseModel, ConfigDict, AliasGenerator
from pydantic.alias_generators import to_camel


class CamelBaseModel(BaseModel):
    model_config = ConfigDict(
        alias_generator=AliasGenerator(
            validation_alias=to_camel,
            serialization_alias=to_camel,
        ),
        populate_by_name=True,
    )


class OptionCreate(CamelBaseModel):
    value: int
    label: str
    is_custom: bool = False


class Option(OptionCreate):
    id: int
    question_id: int

    model_config = ConfigDict(from_attributes=True)


class QuestionCreate(CamelBaseModel):
    title: str
    selected_option: Optional[int] = None
    custom_answer: Optional[str] = None
    options: List[OptionCreate]


class Question(CamelBaseModel):
    title: str
    selected_option: Optional[int]
    custom_answer: Optional[str]
    options: List[Option]
    id: int
    template_reference_id: str

    model_config = ConfigDict(from_attributes=True)


class QuestionTemplateBase(CamelBaseModel):
    template_id: str


class QuestionTemplateCreate(QuestionTemplateBase):
    title: str
    description: str
    created_at: datetime
    questions: List[QuestionCreate]


class QuestionTemplateUpdate(QuestionTemplateCreate):
    pass


class QuestionTemplate(QuestionTemplateBase):
    title: str
    description: str
    created_at: datetime
    questions: List[Question]

    model_config = ConfigDict(from_attributes=True)
