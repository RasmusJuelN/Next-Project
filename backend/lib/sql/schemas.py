from datetime import datetime
from typing import List, Optional
from pydantic import BaseModel, ConfigDict, AliasGenerator
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

    Applies camelCase aliasing to the model attributes, and allows for the model to be populated from attributes. Inherits from CamelBaseModel.

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
    Pydantic model for creating an Option object. Inherits from OptionBase.

    Attributes:
        value (int): The value of the option.
        label (str): The label of the option.
        is_custom (bool): A boolean indicating whether the option is a custom answer. Defaults to False.
    """

    pass


class UpdateOptionModel(BaseOptionModel):
    """
    Pydantic model for updating an Option object. Inherits from OptionBase.

    Attributes:
        value (int): The value of the option.
        label (str): The label of the option.
        is_custom (bool): A boolean indicating whether the option is a custom answer. Defaults to False.
    """

    pass


class OptionModel(BaseOptionModel):
    """
    Pydantic model for an Option object. Inherits from OptionBase.

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

    Applies camelCase aliasing to the model attributes, and allows for the model to be populated from attributes. Inherits from CamelBaseModel.

    Attributes:
        title (str): The title of the question.
        selected_option (int): The value of the selected option. Defaults to None.
        custom_answer (str): The custom answer provided by the user. Defaults to None.
    """

    title: str
    selected_option: Optional[int] = None
    custom_answer: Optional[str] = None


class CreateQuestionModel(BaseQuestionModel):
    """
    Pydantic model for creating a Question object. Inherits from QuestionBase.

    Attributes:
        title (str): The title of the question.
        selected_option (int): The value of the selected option. Defaults to None.
        custom_answer (str): The custom answer provided by the user. Defaults to None.
        options (List[OptionCreate]): A list of OptionCreate objects.
    """

    options: List[CreateOptionModel]


class UpdateQuestionModel(BaseQuestionModel):
    """
    Pydantic model for updating a Question object. Inherits from QuestionBase.

    Attributes:
        title (str): The title of the question.
        selected_option (int): The value of the selected option. Defaults to None.
        custom_answer (str): The custom answer provided by the user. Defaults to None.
        options (List[OptionUpdate]): A list of OptionUpdate objects.
    """

    options: List[UpdateOptionModel]


class QuestionModel(BaseQuestionModel):
    """
    Pydantic model for a Question object. Inherits from QuestionBase.

    Attributes:
        title (str): The title of the question.
        selected_option (int): The value of the selected option. Defaults to None.
        custom_answer (str): The custom answer provided by the user. Defaults to None.
        options (List[Option]): A list of Option objects.
        id (int): The DB autoincrement ID of the question.
    """

    options: List[OptionModel]
    id: int


class BaseQuestionTemplateModel(CamelBaseModel):
    """
    The base Pydantic model for a QuestionTemplate object.

    Applies camelCase aliasing to the model attributes, and allows for the model to be populated from attributes. Inherits from CamelBaseModel.

    Attributes:
        title (str): The title of the template.
        description (str): The description of the template.
        created_at (datetime): The creation date of the template.
    """

    title: str
    description: str
    created_at: datetime


class FetchQuestionTemplateModel(CamelBaseModel):
    """
    Pydantic model for fetching a QuestionTemplate object. Inherits from QuestionTemplateBase.

    Attributes:
        template_id (str): The ID of the template to fetch.
    """

    template_id: str

    model_config = ConfigDict(from_attributes=True)


class DeleteQuestionTemplateModel(CamelBaseModel):
    """
    Pydantic model for deleting a QuestionTemplate object. Inherits from QuestionTemplateBase.

    Attributes:
        template_id (str): The ID of the template to delete.
    """

    template_id: str

    model_config = ConfigDict(from_attributes=True)


class CreateQuestionTemplateModel(BaseQuestionTemplateModel):
    """
    Pydantic model for creating a QuestionTemplate object. Inherits from QuestionTemplateBase.

    Attributes:
        title (str): The title of the template.
        description (str): The description of the template.
        created_at (datetime): The creation date of the template.
        questions (List[QuestionCreate]): A list of QuestionCreate objects.
    """

    questions: List[CreateQuestionModel]


class UpdateQuestionTemplateModel(BaseQuestionTemplateModel):
    """
    Pydantic model for updating a QuestionTemplate object. Inherits from QuestionTemplateBase.

    Attributes:
        title (str): The title of the template.
        description (str): The description of the template.
        created_at (datetime): The creation date of the template.
        questions (List[QuestionUpdate]): A list of QuestionUpdate objects.
    """

    questions: List[UpdateQuestionModel]


class QuestionTemplateModel(BaseQuestionTemplateModel):
    """
    Pydantic model for a QuestionTemplate object. Inherits from QuestionTemplateBase.

    Attributes:
        title (str): The title of the template.
        description (str): The description of the template.
        created_at (datetime): The creation date of the template.
        questions (List[Question]): A list of Question objects.
        id (int): The DB autoincrement ID of the template.
        template_id (str): A unique, server-generated ID for the template.
    """

    questions: List[QuestionModel]
    template_id: str


class QuestionnaireTemplateModel(CamelBaseModel):
    template_id: str
    title: str
    description: str


class ActiveQuestionnaireCreateModel(CamelBaseModel):
    student: User
    teacher: User
    template_id: str


class ActiveQuestionnaireModel(CamelBaseModel):
    id: str
    student: User
    teacher: User
    is_student_finished: bool
    is_teacher_finished: bool
    questionnaire_template: QuestionnaireTemplateModel
    created_at: datetime
