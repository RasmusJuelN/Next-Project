from typing import Tuple, Optional, Sequence, List, overload
from sqlalchemy import Result, select
from sqlalchemy.orm import Session

from backend.lib.sql import schemas, models
from backend.lib.sql.exceptions import (
    TemplateNotFoundException,
    TemplateIdMismatchException,
    TemplateAlreadyExistsException,
)


@overload
def get_template_by_id(
    db: Session, *, template: schemas.QuestionTemplateBase
) -> Optional[schemas.QuestionTemplate]:
    """
    Retrieve a question template by its ID from the database.

    Args:
        db (Session): The database session to use for the query.
        template (schemas.QuestionTemplateBase): The template schema containing the ID of the template to retrieve.

    Returns:
        Optional[schemas.QuestionTemplate]: The question template if found, otherwise None.
    """
    ...


@overload
def get_template_by_id(
    db: Session, *, template_id: str
) -> Optional[schemas.QuestionTemplate]:
    """
    Retrieve a question template by its ID from the database.

    Args:
        db (Session): The database session to use for the query.
        template_id (str): The ID of the template to retrieve.

    Returns:
        Optional[schemas.QuestionTemplate]: The question template if found, otherwise None.
    """
    ...


def get_template_by_id(
    db: Session,
    *,
    template: Optional[schemas.QuestionTemplateBase] = None,
    template_id: Optional[str] = None
) -> Optional[schemas.QuestionTemplate]:
    """
    Retrieve a question template by its ID from the database.

    Args:
        db (Session): The database session to use for the query.
        template (schemas.QuestionTemplateBase): The template schema containing the ID of the template to retrieve.

    Returns:
        Optional[schemas.QuestionTemplate]: The question template if found, otherwise None.
    """
    template_ref_id: str = ""
    if template is not None:
        template_ref_id = template.template_id
    elif template_id is not None:
        template_ref_id = template_id
    else:
        raise ValueError("Either template or template_id must be provided.")

    db.flush()
    result: Result[Tuple[schemas.QuestionTemplate]] = db.execute(
        statement=select(models.QuestionTemplate).where(
            models.QuestionTemplate.template_id == template_ref_id
        )
    )
    return result.scalars().first()


def get_templates(
    db: Session,
) -> Sequence[schemas.QuestionTemplate]:
    """
    Retrieve all question templates from the database.

    Args:
        db (Session): The database session to use for the query.

    Returns:
        Sequence[schemas.QuestionTemplate]: A sequence, typically a list, of all question templates in the database. If no templates are found, an empty sequence is returned.
    """
    db.flush()
    result: Result[Tuple[schemas.QuestionTemplate]] = db.execute(
        statement=select(models.QuestionTemplate)
    )
    return result.scalars().all()


def add_template(
    db: Session, template: schemas.QuestionTemplateCreate
) -> schemas.QuestionTemplate:
    """
    Adds a new question template to the database.

    Args:
        db (Session): The database session to use for the operation.
        template (schemas.QuestionTemplateCreate): The template data to be added.

    Returns:
        schemas.QuestionTemplateCreate: The newly created question template instance.

    Raises:
        TemplateAlreadyExistsException: If a template with the given ID already exists.
    """
    existing_template: Optional[schemas.QuestionTemplate] = get_template_by_id(
        db=db, template=template
    )
    if existing_template:
        raise TemplateAlreadyExistsException(template_id=template.template_id)

    try:
        # start a transaction so that we can rollback if an error occurs
        with db.begin_nested():
            new_template = models.QuestionTemplate(
                template_id=template.template_id,
                title=template.title,
                description=template.description,
                created_at=template.created_at,
                questions=[],
            )

            db.add(instance=new_template)
            db.flush()

        # Create the questions for the template and add them to the database
        new_questions: List[models.Question] = []
        for question in template.questions:
            new_question: models.Question = add_question(
                db=db, question=question, template_id=new_template.template_id
            )
            new_questions.append(new_question)

        # Add the questions to the template. This is only for the response object.
        new_template.questions.extend(new_questions)

        # Create the options for each question and add them to the database
        for index, question in enumerate(iterable=template.questions):
            new_options: List[models.Option] = []
            for option in question.options:
                new_option: models.Option = add_option(
                    db=db, option=option, question_id=new_questions[index].id
                )
                new_options.append(new_option)

            # Add the options to the questions. This is only for the response object.
            new_template.questions[index].options.extend(new_options)

        db.commit()
        db.flush()

    except Exception as e:
        # rollback on error
        db.rollback()
        raise e

    # Return the newly created template
    return schemas.QuestionTemplate.model_validate(obj=new_template)


def update_template(
    db: Session, template: schemas.QuestionTemplateUpdate
) -> schemas.QuestionTemplate:
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
    updated_template: Optional[schemas.QuestionTemplate] = get_template_by_id(
        db=db, template=template
    )
    if not updated_template:
        raise TemplateNotFoundException(template_id=template.template_id)

    updated_template.title = template.title
    updated_template.description = template.description
    updated_template.created_at = template.created_at
    db.commit()
    db.flush()
    return updated_template


def delete_template(
    db: Session, template: schemas.QuestionTemplateBase
) -> schemas.QuestionTemplate:
    """
    Deletes a question template from the database.

    This function deletes a question template identified by the given template data.
    It first flushes the current state of the database session, retrieves the template
    by its ID, and raises an exception if the template is not found. If the template
    exists, it is deleted from the database and the session is committed.

    Args:
        db (Session): The database session to use for the operation.
        template (schemas.QuestionTemplateBase): The template data containing the ID of the template to delete.

    Returns:
        models.QuestionTemplate: The deleted question template.

    Raises:
        TemplateNotFoundException: If the template with the given ID is not found.
    """
    db.flush()
    deleted_template: Optional[schemas.QuestionTemplate] = get_template_by_id(
        db=db, template=template
    )
    if not deleted_template:
        raise TemplateNotFoundException(template_id=template.template_id)

    db.delete(instance=deleted_template)
    db.commit()
    return deleted_template


@overload
def add_question(
    db: Session, question: schemas.QuestionCreate, *, template_id: str
) -> models.Question:
    """
    Adds a new question to the database.

    Args:
        db (Session): The database session to use for the operation.
        question (schemas.QuestionCreate): The question data to be added.
        template_id (str): The ID of the template to which the question belongs.

    Returns:
        models.Question: The newly created question object.
    """
    ...


@overload
def add_question(
    db: Session, question: schemas.QuestionCreate, *, template: models.QuestionTemplate
) -> models.Question:
    """
    Adds a new question to the database.

    Args:
        db (Session): The database session to use for the operation.
        question (schemas.QuestionCreate): The question data to be added.
        template (models.QuestionTemplate): The template which contains the ID to associate the question with.

    Returns:
        models.Question: The newly created question object.
    """
    ...


def add_question(
    db: Session,
    question: schemas.QuestionCreate,
    *,
    template_id: Optional[str] = None,
    template: Optional[models.QuestionTemplate] = None
) -> models.Question:
    """
    Adds a new question to the database.

    Accepts either a template ID or a template object to associate the question with.

    Overload 1:
        db (Session): The database session to use for the operation.
        question (schemas.QuestionCreate): The question data to be added.
        template_id (Optional[str]): The ID of the template to associate the question with.

    Overload 2:
        db (Session): The database session to use for the operation.
        question (schemas.QuestionCreate): The question data to be added.
        template (Optional[models.QuestionTemplate]): The template which contains the ID to associate the question with.

    Raises:
        ValueError: If neither template nor template_id is provided.

    Returns:
        models.Question: The newly created question instance.
    """
    template_ref_id: str = ""
    if template is not None:
        template_ref_id = template.template_id
    elif template_id is not None:
        template_ref_id = template_id
    else:
        raise ValueError("Either template or template_id must be provided.")

    new_question = models.Question(
        template_reference_id=template_ref_id,
        title=question.title,
        selected_option=question.selected_option,
        custom_answer=question.custom_answer,
        options=[],
    )

    db.add(instance=new_question)
    db.flush()

    return new_question


@overload
def add_option(
    db: Session, option: schemas.OptionCreate, *, question_id: int
) -> models.Option:
    """
    Adds a new option to the database.

    Args:
        db (Session): The database session to use for the operation.
        option (schemas.OptionCreate): The option data to be added.
        question_id (int): The ID of the question to which the option belongs.

    Returns:
        models.Option: The newly created option object.
    """
    ...


@overload
def add_option(
    db: Session, option: schemas.OptionCreate, *, question: models.Question
) -> models.Option:
    """
    Adds a new option to the database.

    Args:
        db (Session): The database session to use for the operation.
        option (schemas.OptionCreate): The option data to be added.
        question (models.Question): The question to which the option belongs.

    Returns:
        models.Option: The newly created option object.
    """
    ...


def add_option(
    db: Session,
    option: schemas.OptionCreate,
    *,
    question_id: Optional[int] = None,
    question: Optional[models.Question] = None
) -> models.Option:
    """
    Adds a new option to the database.

    Accepts either a question ID or a question object to associate the option with.

    Overload 1:
        db (Session): The database session to use for the operation.
        option (schemas.OptionCreate): The option data to be added.
        question_id (Optional[int]): The ID of the question to associate the option with.

    Overload 2:
        db (Session): The database session to use for the operation.
        option (schemas.OptionCreate): The option data to be added.
        question (Optional[models.Question]): The question which contains the ID to associate the option with.

    Raises:
        ValueError: If neither question nor question_id is provided.

    Returns:
        models.Option: The newly created option instance.
    """
    question_ref_id: int = -1
    if question is not None:
        question_ref_id = question.id
    elif question_id is not None:
        question_ref_id = question_id
    else:
        raise ValueError("Either question or question_id must be provided.")

    new_option = models.Option(
        question_id=question_ref_id,
        value=option.value,
        label=option.label,
        is_custom=option.is_custom,
    )

    db.add(instance=new_option)
    db.flush()

    return new_option
