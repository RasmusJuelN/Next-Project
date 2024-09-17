from typing import Tuple, Optional, Sequence, List, overload
from sqlalchemy import Result, select
from sqlalchemy.ext.asyncio import AsyncSession

from backend.lib.sql import schemas, models
from backend.lib.sql.exceptions import TemplateNotFoundException


async def get_template_by_id(
    db: AsyncSession, template: schemas.QuestionTemplateBase
) -> Optional[schemas.QuestionTemplate]:
    """
    Retrieve a question template by its ID from the database.

    Args:
        db (AsyncSession): The database session to use for the query.
        template (schemas.QuestionTemplateBase): The template schema containing the ID of the template to retrieve.

    Returns:
        Optional[schemas.QuestionTemplate]: The question template if found, otherwise None.
    """
    await db.flush()
    result: Result[Tuple[schemas.QuestionTemplate]] = await db.execute(
        statement=select(models.QuestionTemplate).where(
            models.QuestionTemplate.templateId == template.templateId
        )
    )
    return result.scalars().first()


async def get_templates(
    db: AsyncSession,
) -> Sequence[schemas.QuestionTemplate]:
    """
    Retrieve all question templates from the database.

    Args:
        db (AsyncSession): The database session to use for the query.

    Returns:
        Sequence[schemas.QuestionTemplate]: A sequence, typically a list, of all question templates in the database. If no templates are found, an empty sequence is returned.
    """
    await db.flush()
    result: Result[Tuple[schemas.QuestionTemplate]] = await db.execute(
        statement=select(models.QuestionTemplate)
    )
    return result.scalars().all()


async def add_template(
    db: AsyncSession, template: schemas.QuestionTemplateCreate
) -> models.QuestionTemplate:
    """
    Asynchronously adds a new question template to the database.

    Args:
        db (AsyncSession): The database session to use for the operation.
        template (schemas.QuestionTemplateCreate): The template data to be added.

    Returns:
        schemas.QuestionTemplateCreate: The newly created question template instance.
    """
    new_template = models.QuestionTemplate(
        templateId=template.templateId,
        title=template.title,
        description=template.description,
        createdAt=template.createdAt,
    )

    try:
        # start a transaction so that we can rollback if an error occurs
        async with db.begin():

            db.add(instance=new_template)
            await db.flush()

        # Create the questions for the template and add them to the database
        new_questions: List[models.Question] = []
        for question in template.questions:
            new_question: models.Question = await add_question(
                db=db, question=question, template_id=new_template.templateId
            )
            new_questions.append(new_question)

        # Add the questions to the template. This is only for the response object.
        new_template.questions.extend(new_questions)

        # Create the options for each question and add them to the database
        for index, question in enumerate(iterable=template.questions):
            new_options: List[models.Option] = []
            for option in question.options:
                new_option: models.Option = await add_option(
                    db=db, option=option, question_id=new_questions[index].id
                )
                new_options.append(new_option)

            # Add the options to the questions. This is only for the response object.
            new_template.questions[index].options.extend(new_options)

        await db.commit()
        await db.flush()

    except Exception as e:
        # rollback on error
        await db.rollback()
        raise e

    # Return the newly created template
    return new_template


async def update_template(
    db: AsyncSession, template: schemas.QuestionTemplateUpdate
) -> schemas.QuestionTemplate:
    """
    Update an existing question template in the database.

    Args:
        db (AsyncSession): The database session.
        template (schemas.QuestionTemplateUpdate): The template data to update.

    Returns:
        models.QuestionTemplate: The updated question template.

    Raises:
        TemplateNotFoundException: If the template with the given ID does not exist.
    """
    updated_template: Optional[schemas.QuestionTemplate] = await get_template_by_id(
        db=db, template=template
    )
    if not updated_template:
        raise TemplateNotFoundException(template_id=template.templateId)

    updated_template.title = template.title
    updated_template.description = template.description
    updated_template.createdAt = template.createdAt
    await db.commit()
    await db.flush()
    return updated_template


async def delete_template(
    db: AsyncSession, template: schemas.QuestionTemplateBase
) -> schemas.QuestionTemplate:
    """
    Deletes a question template from the database.

    This function deletes a question template identified by the given template data.
    It first flushes the current state of the database session, retrieves the template
    by its ID, and raises an exception if the template is not found. If the template
    exists, it is deleted from the database and the session is committed.

    Args:
        db (AsyncSession): The database session to use for the operation.
        template (schemas.QuestionTemplateBase): The template data containing the ID of the template to delete.

    Returns:
        models.QuestionTemplate: The deleted question template.

    Raises:
        TemplateNotFoundException: If the template with the given ID is not found.
    """
    await db.flush()
    deleted_template: Optional[schemas.QuestionTemplate] = await get_template_by_id(
        db=db, template=template
    )
    if not deleted_template:
        raise TemplateNotFoundException(template_id=template.templateId)

    await db.delete(instance=deleted_template)
    await db.commit()
    return deleted_template


@overload
async def add_question(
    db: AsyncSession, question: schemas.QuestionCreate, *, template_id: str
) -> models.Question:
    """
    Asynchronously adds a new question to the database.

    Args:
        db (AsyncSession): The database session to use for the operation.
        question (schemas.QuestionCreate): The question data to be added.
        template_id (str): The ID of the template to which the question belongs.

    Returns:
        models.Question: The newly created question object.
    """
    ...


@overload
async def add_question(
    db: AsyncSession,
    question: schemas.QuestionCreate,
    *,
    template: models.QuestionTemplate
) -> models.Question:
    """
    Asynchronously adds a new question to the database.

    Args:
        db (AsyncSession): The database session to use for the operation.
        question (schemas.QuestionCreate): The question data to be added.
        template (models.QuestionTemplate): The template which contains the ID to associate the question with.

    Returns:
        models.Question: The newly created question object.
    """
    ...


async def add_question(
    db: AsyncSession,
    question: schemas.QuestionCreate,
    *,
    template_id: Optional[str] = None,
    template: Optional[models.QuestionTemplate] = None
) -> models.Question:
    """
    Asynchronously adds a new question to the database.

    Accepts either a template ID or a template object to associate the question with.

    Overload 1:
        db (AsyncSession): The database session to use for the operation.
        question (schemas.QuestionCreate): The question data to be added.
        template_id (Optional[str]): The ID of the template to associate the question with.

    Overload 2:
        db (AsyncSession): The database session to use for the operation.
        question (schemas.QuestionCreate): The question data to be added.
        template (Optional[models.QuestionTemplate]): The template which contains the ID to associate the question with.

    Raises:
        ValueError: If neither template nor template_id is provided.

    Returns:
        models.Question: The newly created question instance.
    """
    template_ref_id: str = ""
    if template is not None:
        template_ref_id = template.templateId
    elif template_id is not None:
        template_ref_id = template_id
    else:
        raise ValueError("Either template or template_id must be provided.")

    new_question = models.Question(
        templateReferenceId=template_ref_id,
        title=question.title,
        selectedOption=question.selectedOption,
        customAnswer=question.customAnswer,
    )

    db.add(instance=new_question)
    await db.flush()

    return new_question


@overload
async def add_option(
    db: AsyncSession, option: schemas.OptionCreate, *, question_id: int
) -> models.Option:
    """
    Asynchronously adds a new option to the database.

    Args:
        db (AsyncSession): The database session to use for the operation.
        option (schemas.OptionCreate): The option data to be added.
        question_id (int): The ID of the question to which the option belongs.

    Returns:
        models.Option: The newly created option object.
    """
    ...


@overload
async def add_option(
    db: AsyncSession, option: schemas.OptionCreate, *, question: models.Question
) -> models.Option:
    """
    Asynchronously adds a new option to the database.

    Args:
        db (AsyncSession): The database session to use for the operation.
        option (schemas.OptionCreate): The option data to be added.
        question (models.Question): The question to which the option belongs.

    Returns:
        models.Option: The newly created option object.
    """
    ...


async def add_option(
    db: AsyncSession,
    option: schemas.OptionCreate,
    *,
    question_id: Optional[int] = None,
    question: Optional[models.Question] = None
) -> models.Option:
    """
    Asynchronously adds a new option to the database.

    Accepts either a question ID or a question object to associate the option with.

    Overload 1:
        db (AsyncSession): The database session to use for the operation.
        option (schemas.OptionCreate): The option data to be added.
        question_id (Optional[int]): The ID of the question to associate the option with.

    Overload 2:
        db (AsyncSession): The database session to use for the operation.
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
        questionId=question_ref_id,
        value=option.value,
        label=option.label,
        isCustom=option.isCustom,
    )

    db.add(instance=new_option)
    await db.flush()

    return new_option
