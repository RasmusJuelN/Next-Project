from datetime import datetime
from typing import List, Optional, Self
from pydantic import BaseModel, ConfigDict, model_validator


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

    # Validate that either selectedOption or customAnswer is provided, but not both
    @model_validator(mode="after")
    def validate_selected_or_custom(self) -> Self:
        selected_option: Optional[int] = self.selected_option
        custom_answer: Optional[str] = self.custom_answer

        if selected_option is None and custom_answer is None:
            raise ValueError("Either selectedOption or customAnswer must be provided.")
        elif selected_option is not None and custom_answer is not None:
            raise ValueError("Both selectedOption and customAnswer cannot be provided.")
        return self


class QuestionCreate(QuestionBase):
    options: List[OptionCreate]


class QuestionUpdate(QuestionBase):
    options: List[OptionUpdate]


class Question(QuestionCreate):
    model_config = ConfigDict(from_attributes=True)

    id: int
    template_reference_id: str


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
