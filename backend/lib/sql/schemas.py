from datetime import datetime
from typing import List, Optional, Self
from pydantic import BaseModel, ConfigDict, model_validator


class OptionBase(BaseModel):
    value: int
    label: str
    isCustom: bool = False


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
    selectedOption: Optional[int] = None
    customAnswer: Optional[str] = None


class QuestionCreate(QuestionBase):
    options: List[OptionCreate]


class QuestionUpdate(QuestionBase):
    options: List[OptionUpdate]


class Question(QuestionCreate):
    model_config = ConfigDict(from_attributes=True)

    id: int
    template_reference_id: str


class QuestionTemplateBase(BaseModel):
    templateId: str


class QuestionTemplateCreate(QuestionTemplateBase):
    title: str
    description: str
    questions: List[QuestionCreate]
    createdAt: datetime


class QuestionTemplateUpdate(QuestionTemplateCreate):
    pass


class QuestionTemplate(QuestionTemplateCreate):
    model_config = ConfigDict(from_attributes=True)

    id: int
