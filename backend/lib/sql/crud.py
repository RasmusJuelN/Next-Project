from typing import Tuple, Optional, Sequence, Union, Protocol, TypeAlias
from sqlalchemy import Result, select, and_
from sqlalchemy.orm import Session

from backend.lib.sql import schemas, models
from backend.lib.sql.utils import (
    check_if_record_exists_by_id,
    user_id_condition,
    student_name_condition,
    teacher_name_condition,
    create_option_model,
    create_question_model,
)
from backend.lib.sql.exceptions import (
    TemplateNotFoundException,
    QuestionnaireNotFound,
)


class ObjectHasTemplateID(Protocol):
    id: str


HasID: TypeAlias = Union[ObjectHasTemplateID, str]


def get_template_by_id(
    db: Session,
    template_id: HasID,
) -> Optional[models.QuestionTemplate]:
    """
    Retrieve a question template by its ID from the database.

    Args:
        db (Session): The database session to use for the query.
        template_id (HasID): The ID of the template to retrieve.

    Returns:
        Optional[models.QuestionTemplate]: The template if found, otherwise None.
    """
    if not isinstance(template_id, str):
        template_id = template_id.id

    with db.begin_nested():
        result: Result[Tuple[models.QuestionTemplate]] = db.execute(
            statement=select(models.QuestionTemplate).where(
                models.QuestionTemplate.id == template_id
            )
        )
        return result.scalars().first()


def get_all_templates(
    db: Session,
) -> Sequence[models.QuestionTemplate]:
    """
    Retrieve all question templates from the database.

    Args:
        db (Session): The database session to use for the query.

    Returns:
        Sequence[models.QuestionTemplate]: A sequence, typically a
        list, of all question templates in the database. If no templates
        are found, an empty sequence is returned.
    """

    with db.begin_nested():
        result: Result[Tuple[models.QuestionTemplate]] = db.execute(
            statement=select(models.QuestionTemplate)
        )
        return result.scalars().all()


def get_templates_by_title(
    db: Session, title: str
) -> Sequence[models.QuestionTemplate]:
    """
    Retrieve all question templates which contain the given title
    from the database.

    Args:
        db (Session): The database session to use for the query.
        title (str): The title of the templates to retrieve.

    Returns:
        Sequence[models.QuestionTemplate]: A sequence, typically a
        list, of all question templates with the given title. If no templates are found, an empty sequence is returned.
    """

    with db.begin_nested():
        result: Result[Tuple[models.QuestionTemplate]] = db.execute(
            statement=select(models.QuestionTemplate).where(
                models.QuestionTemplate.title.like(other=f"%{title}%")
            )
        )
        return result.scalars().all()


def add_template(
    db: Session, template: schemas.CreateQuestionTemplateModel
) -> models.QuestionTemplate:
    """
    Adds a new question template to the database.

    Args:
        db (Session): The database session to use for the operation.
        template (schemas.QuestionTemplateCreate): The template data to be added.

    Returns:
        models.QuestionTemplate: The newly created question template instance.

    Raises:
        TemplateAlreadyExistsException: If a template with the given ID already exists.
    """
    with db.begin_nested():
        # Create the new template
        new_template = models.QuestionTemplate(
            title=template.title,
            description=template.description,
        )

        # Create the new questions
        for question in template.questions:
            new_question: models.Question = create_question_model(
                schema=question,
                template_id=new_template.id,
            )
            new_template.questions.append(new_question)

        # Create the new options
        for index, question in enumerate(iterable=template.questions):
            for option in question.options:
                new_option: models.Option = create_option_model(
                    schema=option,
                    question_id=new_question.id,
                )
                new_template.questions[index].options.append(new_option)

        # Confused why we aren't adding the individual questions and
        # options to the database, or even filling in the foreign keys?
        # No worries! SQLAlchemy is so smart that it will automatically
        # add the new questions and options, and fill in the foreign keys
        # to the correct template and question IDs when we add the new
        # template to the database.
        db.add(instance=new_template)
        return new_template


def update_template(
    db: Session,
    existing_id: HasID,
    updated_template: schemas.UpdateQuestionTemplateModel,
) -> models.QuestionTemplate:
    """
    Update an existing question template in the database.

    Args:
        db (Session): The database session.
        template (schemas.QuestionTemplateUpdate): The template data to update.

    Returns:
        models.QuestionTemplate: The updated question template.

    Raises:
        TemplateNotFoundException: If the template with the given ID does not exist.
    """
    if not isinstance(existing_id, str):
        existing_id = existing_id.id

    with db.begin_nested():
        existing_template: Optional[models.QuestionTemplate] = get_template_by_id(
            db=db, template_id=existing_id
        )
        if not existing_template:
            raise TemplateNotFoundException(template_id=existing_id)

        # Update the base template data
        existing_template.title = updated_template.title
        existing_template.description = updated_template.description

        # Update the existing questions
        try:
            for existing_question, updated_question in zip(
                existing_template.questions, updated_template.questions, strict=True
            ):
                existing_question.title = updated_question.title

                # Update the existing options for the current question
                try:
                    for existing_option, updated_option in zip(
                        existing_question.options,
                        updated_question.options,
                        strict=True,
                    ):
                        existing_option.value = updated_option.value
                        existing_option.label = updated_option.label
                        existing_option.is_custom = updated_option.is_custom
                except ValueError:
                    existing_question = add_or_remove_options(
                        existing_question=existing_question,
                        updated_question=updated_question,
                    )

        except ValueError:
            existing_template = add_or_remove_questions(
                existing_template=existing_template,
                updated_template=updated_template,
            )

        db.add(instance=existing_template)

        return existing_template


def delete_template(db: Session, template: HasID) -> models.QuestionTemplate:
    """
    Deletes a template from the database.

    Args:
        db (Session): The database session to use for the operation.
        template (HasID): The template to delete, identified by its ID.

    Returns:
        models.QuestionTemplate: The deleted template object.

    Raises:
        TemplateNotFoundException: If the template with the given ID does not exist.
    """
    with db.begin_nested():
        template_to_delete: Optional[models.QuestionTemplate] = get_template_by_id(
            db=db, template_id=template
        )
        if not template_to_delete:
            if isinstance(template, str):
                raise TemplateNotFoundException(template_id=template)
            else:
                raise TemplateNotFoundException(template_id=template.id)

        db.delete(instance=template_to_delete)

        return template_to_delete


def get_option_by_id(db: Session, option_id: int) -> Optional[schemas.OptionModel]:
    """
    Retrieve an option by its ID from the database.

    Args:
        db (Session): The database session to use for the query.
        option_id (int): The ID of the option to retrieve.

    Returns:
        Optional[schemas.OptionModel]: The option if found, otherwise None.
    """

    with db.begin_nested():
        result: Result[Tuple[schemas.OptionModel]] = db.execute(
            statement=select(models.Option).where(models.Option.id == option_id)
        )
        return result.scalars().first()


def get_options_by_question_id(
    db: Session, question_id: int
) -> Sequence[schemas.OptionModel]:
    """
    Retrieve all options for a given question from the database.

    Args:
        db (Session): The database session to use for the query.
        question_id (int): The ID of the question to retrieve options for.

    Returns:
        Sequence[schemas.OptionModel]: A sequence, typically a list, of all options for the given question. If no options are found, an empty sequence is returned.
    """

    with db.begin_nested():
        result: Result[Tuple[schemas.OptionModel]] = db.execute(
            statement=select(models.Option).where(
                models.Option.question_id == question_id
            )
        )
        return result.scalars().all()


def add_active_questionnaire(
    db: Session,
    questionnaire: schemas.ActiveQuestionnaireCreateModel,
) -> models.ActiveQuestionnaire:
    """
    Adds a new active questionnaire to the database. If the student or teacher
    associated with the questionnaire does not exist in the database, they are
    also added.

    Args:
        db (Session): The database session to use for the operation.
        questionnaire (schemas.ActiveQuestionnaireCreateModel): The questionnaire
            data to be added.

    Returns:
        models.ActiveQuestionnaire: The newly created active questionnaire record.
    """
    with db.begin_nested():
        new_active_questionnaire = models.ActiveQuestionnaire(
            student_id=questionnaire.student.id,
            teacher_id=questionnaire.teacher.id,
            template_reference_id=questionnaire.id,
        )

        db.add(instance=new_active_questionnaire)

        # Check if the student exists in the database, if not add them
        if not check_if_record_exists_by_id(
            db=db, model=models.User, id=questionnaire.student.id
        ):
            new_student = models.User(
                id=questionnaire.student.id,
                user_name=questionnaire.student.user_name,
                full_name=questionnaire.student.full_name,
                role=questionnaire.student.role,
            )
            db.add(instance=new_student)

        # Check if the teacher exists in the database, if not add them
        if not check_if_record_exists_by_id(
            db=db, model=models.User, id=questionnaire.teacher.id
        ):
            new_teacher = models.User(
                id=questionnaire.teacher.id,
                user_name=questionnaire.teacher.user_name,
                full_name=questionnaire.teacher.full_name,
                role=questionnaire.teacher.role,
            )
            db.add(instance=new_teacher)

        return new_active_questionnaire


def get_all_active_questionnaires(
    db: Session,
    teacher: str,
    student: str,
) -> Sequence[models.ActiveQuestionnaire]:
    """
    Retrieve all active questionnaires for a specific teacher and student.

    Args:
        db (Session): The database session to use for the query.
        teacher (str): The name of the teacher associated with the questionnaires.
        student (str): The name of the student associated with the questionnaires.

    Returns:
        Sequence[models.ActiveQuestionnaire]: A list of active questionnaires that match the given teacher and student.
    """

    with db.begin_nested():
        result: Result[Tuple[models.ActiveQuestionnaire]] = db.execute(
            statement=select(models.ActiveQuestionnaire).where(
                and_(
                    student_name_condition(student_name=student),
                    teacher_name_condition(teacher_name=teacher),
                )
            )
        )
        return result.scalars().all()


def get_active_questionnaire_by_id(
    db: Session,
    questionnaire_id: str,
) -> Optional[models.ActiveQuestionnaire]:
    """
    Retrieve an active questionnaire by its ID from the database.

    Args:
        db (Session): The database session to use for the query.
        questionnaire_id (str): The ID of the questionnaire to retrieve.

    Returns:
        Optional[models.ActiveQuestionnaire]: The active questionnaire if found, otherwise None.
    """

    with db.begin_nested():
        result: Result[Tuple[models.ActiveQuestionnaire]] = db.execute(
            statement=select(models.ActiveQuestionnaire).where(
                models.ActiveQuestionnaire.id == questionnaire_id
            )
        )
        return result.scalars().first()


def get_oldest_active_questionnaire_id_for_user(
    db: Session,
    user_id: str,
) -> Optional[str]:
    """
    Retrieve the ID of the oldest active questionnaire for a given user.

    Args:
        db (Session): The database session to use for the query.
        user_id (str): The ID of the user for whom to retrieve the oldest active questionnaire.

    Returns:
        Optional[str]: The ID of the oldest active questionnaire if found, otherwise None.
    """

    with db.begin_nested():
        result: Result[Tuple[str]] = db.execute(
            statement=select(models.ActiveQuestionnaire.id)
            .where(user_id_condition(user_id=user_id))
            .order_by(models.ActiveQuestionnaire.created_at)
        )
        return result.scalars().first()


def get_all_active_questionnaire_ids_for_user(
    db: Session,
    user_id: str,
) -> Sequence[str]:
    """
    Retrieve all active questionnaire IDs for a given user.

    Args:
        db (Session): The database session to use for the query.
        user_id (str): The ID of the user for whom to retrieve active questionnaire IDs.

    Returns:
        Sequence[str]: A sequence of active questionnaire IDs for the specified user.
    """

    with db.begin_nested():
        result: Result[Tuple[str]] = db.execute(
            statement=select(models.ActiveQuestionnaire.id)
            .where(user_id_condition(user_id=user_id))
            .order_by(models.ActiveQuestionnaire.created_at)
        )
        return result.scalars().all()


def delete_active_questionnaire(
    db: Session,
    questionnaire_id: str,
) -> models.ActiveQuestionnaire:
    """
    Deletes an active questionnaire from the database.

    Args:
        db (Session): The database session to use for the operation.
        questionnaire_id (str): The ID of the questionnaire to delete.

    Returns:
        models.ActiveQuestionnaire: The deleted active questionnaire object.

    Raises:
        QuestionnaireNotFound: If no active questionnaire is found with the given ID.
    """

    with db.begin_nested():
        questionnaire_to_delete: Optional[models.ActiveQuestionnaire] = (
            get_active_questionnaire_by_id(db=db, questionnaire_id=questionnaire_id)
        )
        if not questionnaire_to_delete:
            raise QuestionnaireNotFound(questionnaire_id=questionnaire_id)

        db.delete(instance=questionnaire_to_delete)
        return questionnaire_to_delete


def add_or_remove_options(
    existing_question: models.Question,
    updated_question: schemas.UpdateQuestionModel,
) -> models.Question:
    """
    Add or remove options for a given question based on the updated question model.

    If the number of options in the existing question is greater than the number of options
    in the updated question, the extra options are deleted. If the number of options in the
    existing question is less than the number of options in the updated question, the missing
    options are added. If the number of options in the existing question is equal to the number
    of options in the updated question, the options are returned as is.

    Args:
        existing_question (models.Question): The existing question object from the database.
        updated_question (schemas.UpdateQuestionModel): The updated question model containing
            the new set of options.

    Returns:
        models.Question: The updated question object with the modified options.
    """
    if len(existing_question.options) > len(updated_question.options):
        # Delete any extra options
        for _ in range(
            len(updated_question.options),
            len(existing_question.options),
        ):
            existing_question.options.remove(existing_question.options[-1])
        return existing_question
    elif len(existing_question.options) < len(updated_question.options):
        # Add any missing options
        extra_options: int = len(existing_question.options)
        for option in updated_question.options[extra_options:]:
            new_option: models.Option = create_option_model(
                schema=option,
                question_id=existing_question.id,
            )
            existing_question.options.append(new_option)
        return existing_question
    else:
        return existing_question


def add_or_remove_questions(
    existing_template: models.QuestionTemplate,
    updated_template: schemas.UpdateQuestionTemplateModel,
) -> models.QuestionTemplate:
    """
    Add or remove questions from an existing question template based on an updated template.

    If the updated template has fewer questions than the existing template, the extra questions
    in the existing template will be deleted. If the updated template has more questions, the
    additional questions will be added to the existing template. If the updated template has the
    same number of questions, the questions will be returned as is.

    Args:
        existing_template (models.QuestionTemplate): The current question template.
        updated_template (schemas.UpdateQuestionTemplateModel): The updated question template model.

    Returns:
        models.QuestionTemplate: The updated question template with questions added or removed.
    """
    if len(existing_template.questions) > len(updated_template.questions):
        # Delete any extra questions
        for _ in range(
            len(updated_template.questions),
            len(existing_template.questions),
        ):
            existing_template.questions.remove(existing_template.questions[-1])
        return existing_template
    elif len(existing_template.questions) < len(updated_template.questions):
        # Add any missing questions
        extra_questions: int = len(existing_template.questions)
        for question in updated_template.questions[extra_questions:]:
            new_question: models.Question = create_question_model(
                schema=question,
                template_id=existing_template.id,
            )
            existing_template.questions.append(new_question)
        return existing_template
    else:
        return existing_template
