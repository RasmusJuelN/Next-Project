from datetime import datetime
from typing import List, Optional, Self
from pydantic import BaseModel, ConfigDict, AliasGenerator, model_validator
from pydantic.alias_generators import to_camel

from backend.lib.api.auth.models import User


class CamelBaseModel(BaseModel):
    model_config = ConfigDict(
        alias_generator=AliasGenerator(
            validation_alias=to_camel,
            serialization_alias=to_camel,
        ),
        populate_by_name=True,
        from_attributes=True,
    )


class BaseOptionModel(CamelBaseModel):
    """
    The base Pydantic model for an Option object.

    Applies camelCase aliasing to the model attributes, and allows for the model to be populated from attributes. Extends CamelBaseModel.

    Attributes:
        value (int): The value of the option.
        label (str): The label of the option.
        is_custom (bool): A boolean indicating whether the option is a custom answer. Defaults to False.
    """

    value: int
    label: str
    is_custom: bool = False


class CreateOptionModel(BaseOptionModel):
    """
    Pydantic model for creating an Option object. Extends BaseOptionModel.

    Attributes:
        value (int): The value of the option.
        label (str): The label of the option.
        is_custom (bool): A boolean indicating whether the option is a custom answer. Defaults to False.
    """

    pass


class UpdateOptionModel(BaseOptionModel):
    """
    Pydantic model for updating an Option object. Extends BaseOptionModel.

    Attributes:
        value (int): The value of the option.
        label (str): The label of the option.
        is_custom (bool): A boolean indicating whether the option is a custom answer. Defaults to False.
    """

    pass


class OptionModel(BaseOptionModel):
    """
    Pydantic model for an Option object. Extends BaseOptionModel.
    Should be a 1:1 representation of the Option object in the database.

    Attributes:
        value (int): The value of the option.
        label (str): The label of the option.
        is_custom (bool): A boolean indicating whether the option is a custom answer. Defaults to False.
        id (int): The DB autoincrement ID of the option.
    """

    id: int


class BaseQuestionModel(CamelBaseModel):
    """
    The base Pydantic model for a Question object.

    Applies camelCase aliasing to the model attributes, and allows for the model to be populated from attributes. Extends CamelBaseModel.

    Attributes:
        title (str): The title of the question.
    """

    title: str


class CreateQuestionModel(BaseQuestionModel):
    """
    Pydantic model for creating a Question object. Extends QuestionBase.

    Attributes:
        title (str): The title of the question.
        options (List[OptionCreate]): A list of OptionCreate objects.
    """

    options: List[CreateOptionModel]


class UpdateQuestionModel(BaseQuestionModel):
    """
    Pydantic model for updating a Question object. Extends QuestionBase.

    Attributes:
        title (str): The title of the question.
        options (List[OptionUpdate]): A list of OptionUpdate objects.
    """

    options: List[UpdateOptionModel]


class QuestionModel(BaseQuestionModel):
    """
    Pydantic model for a Question object. Extends QuestionBase.
    Should be a 1:1 representation of the Question object in the database.

    Attributes:
        title (str): The title of the question.
        options (List[Option]): A list of Option objects.
        id (int): The DB autoincrement ID of the question.
    """

    options: List[OptionModel]
    id: int


class BaseQuestionTemplateModel(CamelBaseModel):
    """
    The base Pydantic model for a QuestionTemplate object.

    Applies camelCase aliasing to the model attributes, and allows for the model to be populated from attributes. Extends CamelBaseModel.

    Attributes:
        title (str): The title of the template.
        description (str): The description of the template.
    """

    title: str
    description: str


class FetchQuestionTemplateModel(CamelBaseModel):
    """
    Pydantic model for fetching a QuestionTemplate object. Extends BaseQuestionTemplateModel.

    Attributes:
        created_at (datetime): The creation date of the template.
        last_updated (datetime): The last update date of the template.
        id (str): The ID of the template to fetch.
    """

    created_at: datetime
    last_updated: datetime
    id: str


class DeleteQuestionTemplateModel(CamelBaseModel):
    """
    Pydantic model for deleting a QuestionTemplate object. Extends BaseQuestionTemplateModel.

    Attributes:
        created_at (Optional[datetime]): The creation date of the template.
            If set, the template will only be deleted if the creation date matches.
        last_updated (Optional[datetime]): The last update date of the template.
            If set, the template will only be deleted if the last update date matches.
        id (str): The ID of the template to delete.
    """

    created_at: Optional[datetime] = None
    last_updated: Optional[datetime] = None
    id: str


class CreateQuestionTemplateModel(BaseQuestionTemplateModel):
    """
    Pydantic model for creating a QuestionTemplate object. Extends BaseQuestionTemplateModel.

    Attributes:
        title (str): The title of the template.
        description (str): The description of the template.
        questions (List[CreateQuestionModel]): A list of CreateQuestionModel objects.
    """

    questions: List[CreateQuestionModel]


class UpdateQuestionTemplateModel(BaseQuestionTemplateModel):
    """
    Pydantic model for updating a QuestionTemplate object. Extends BaseQuestionTemplateModel.

    Attributes:
        title (str): The title of the template.
        description (str): The description of the template.
        questions (List[QuestionUpdate]): A list of QuestionUpdate objects.
    """

    questions: List[UpdateQuestionModel]


class QuestionTemplateModel(BaseQuestionTemplateModel):
    """
    Pydantic model for a QuestionTemplate object. Extends BaseQuestionTemplateModel.
    Should be a 1:1 representation of the QuestionTemplate object in the database.

    Attributes:
        title (str): The title of the template.
        description (str): The description of the template.
        questions (List[Question]): A list of Question objects.
        id (str): A unique, server-generated ID for the template.
    """

    questions: List[QuestionModel]
    id: str


class QuestionnaireTemplateModel(CamelBaseModel):
    """
    Pydantic model for a QuestionTemplateModel which has been stripped of its questions.
    Extends CamelBaseModel.

    Attributes:
        id (str): The ID of the template.
        title (str): The title of the template.
        description (str): The description of the template.
    """

    id: str
    title: str
    description: str


class ActiveQuestionnaireCreateModel(CamelBaseModel):
    """
    Pydantic model for creating an ActiveQuestionnaire object. Extends CamelBaseModel.

    Attributes:
        student (User): The student who is taking the questionnaire.
        teacher (User): The teacher who is taking the questionnaire.
        template (QuestionnaireTemplateModel): The template of the questionnaire.
    """

    student: User
    teacher: User
    id: str


class ActiveQuestionnaireModel(CamelBaseModel):
    """
    Pydantic model for an ActiveQuestionnaire object. Extends CamelBaseModel.

    Attributes:
        id (str): The ID of the questionnaire.
        student (User): The student who is taking the questionnaire.
        teacher (User): The teacher who is taking the questionnaire.
        template (QuestionnaireTemplateModel): The template of the questionnaire.
        created_at (datetime): The creation date of the questionnaire.
        student_finished_at (Optional[datetime]): The date the student finished the questionnaire.
        teacher_finished_at (Optional[datetime]): The date the teacher finished the questionnaire.
    """

    id: str
    student: User
    teacher: User
    template: QuestionnaireTemplateModel
    created_at: datetime
    student_finished_at: Optional[datetime] = None
    teacher_finished_at: Optional[datetime] = None


class TeacherStudentPairModel(CamelBaseModel):
    """
    Pydantic model for a teacher and student pair. Extends CamelBaseModel.

    Attributes:
        teacher (User): The teacher in the pair.
        student (User): The student in the pair.
    """

    teacher: User
    student: User


class AnswerModel(CamelBaseModel):
    """
    Pydantic model for an answer to a question. Extends CamelBaseModel.

    Attributes:
        question_id (int): The ID of the question.
        selected_option_id (Optional[int]): The ID of the selected option. If not provided, custom_answer_text must be provided.
        custom_answer_text (Optional[str]): The custom answer text. If not provided, selected_option_id must be provided.
    """

    question_id: int
    selected_option_id: Optional[int] = 0
    custom_answer_text: Optional[str] = None

    @model_validator(mode="after")
    def validate_answer(self) -> Self:
        if self.selected_option_id is None and self.custom_answer_text is None:
            raise ValueError(
                "Either selected_option_id or custom_answer_text must be provided"
            )
        return self


class AnswerSubmissionModel(CamelBaseModel):
    """
    Pydantic model for submitting answers to a questionnaire. Extends CamelBaseModel.

    Attributes:
        questionnaire_id (str): The ID of the questionnaire.
        user_id (str): The ID of the user submitting the answers.
        answers (List[AnswerModel]): A list of AnswerModel objects.
    """

    questionnaire_id: str
    user_id: str
    answers: List[AnswerModel]


class AnswerResultModel(CamelBaseModel):
    """
    Pydantic model for a result of a question. Extends CamelBaseModel.

    Attributes:
        question_id (int): The ID of the question.
        question_title (str): The title of the question.
        student_answer (str): The answer provided by the student.
        teacher_answer (str): The answer provided by the teacher.
    """

    question_id: int
    question_title: str
    student_answer: str
    teacher_answer: str


class QuestionnaireResultModel(CamelBaseModel):
    """
    Pydantic model for the results of a questionnaire. Extends CamelBaseModel.

    Attributes:
        questionnaire_id (str): The ID of the questionnaire.
        users (TeacherAndUserModel): The teacher and student who took the questionnaire.
        answers (List[AnswerResultModel]): A list of AnswerResultModel objects.
    """

    questionnaire_id: str
    users: TeacherStudentPairModel
    answers: List[AnswerResultModel]
