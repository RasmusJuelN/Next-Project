from datetime import datetime
from typing import List, Optional
from pydantic import BaseModel, ConfigDict


class OptionBase(BaseModel):
    value: int
    label: str
    is_custom: bool


class OptionCreate(OptionBase):
    pass


class OptionUpdate(OptionBase):
    pass


class Option(OptionBase):
    model_config = ConfigDict(from_attributes=True)

    id: int
    question_id: int


class QuestionBase(BaseModel):
    title: str
    selected_option: Optional[int] = None
    custom_answer: Optional[str] = None


class QuestionCreate(QuestionBase):
    options: List[OptionCreate]


class QuestionUpdate(QuestionBase):
    options: List[OptionUpdate]


class Question(QuestionBase):
    model_config = ConfigDict(from_attributes=True)

    id: int
    template_reference_id: str
    options: List[Option]


class QuestionTemplateBase(BaseModel):
    template_id: str


class QuestionTemplateCreate(QuestionTemplateBase):
    title: str
    description: str
    questions: List[QuestionCreate]
    created_at: datetime


class QuestionTemplateUpdate(QuestionTemplateCreate):
    pass


class QuestionTemplate(QuestionTemplateCreate):
    model_config = ConfigDict(from_attributes=True)

    id: int
