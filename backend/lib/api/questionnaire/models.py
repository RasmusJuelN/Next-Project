from pydantic import BaseModel, model_validator
from typing import List, Optional, Self
from datetime import datetime


class Option(BaseModel):
    id: int  # Reference ID corresponding to the option stored in the database
    value: int  # A value indicating the option selected by the user
    label: str  # The text displayed to the user
    isCustom: bool = (
        False  # A flag indicating if the option is a custom answer. Defaults to False as it is optional
    )


class Question(BaseModel):
    id: int  # Reference ID corresponding to the question stored in the database
    title: str  # The text displayed to the user
    options: List[Option]  # A list of options available for the user to select
    selectedOption: Optional[int]  # The value of the option selected by the user
    customAnswer: Optional[
        str
    ]  # The custom answer provided by the user if the selected option is a custom answer

    # Validate that either selectedOption or customAnswer is provided, but not both
    @model_validator(mode="after")
    def validate_selected_or_custom(self) -> Self:
        selected_option: Optional[int] = self.selectedOption
        custom_answer: Optional[str] = self.customAnswer

        if selected_option is None and custom_answer is None:
            raise ValueError("Either selectedOption or customAnswer must be provided.")
        elif selected_option is not None and custom_answer is not None:
            raise ValueError("Both selectedOption and customAnswer cannot be provided.")
        return self


class QuestionTemplate(BaseModel):
    templateId: str  # Reference ID corresponding to the template stored in the database
    title: str  # The title of the questionnaire template
    description: str  # The description of the questionnaire template
    questions: List[Question]  # A list of questions in the questionnaire template
    createdAt: datetime  # The timestamp indicating when the template was created.
