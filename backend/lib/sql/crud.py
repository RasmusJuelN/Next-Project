from typing import Tuple, Optional, Sequence, List
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
    new_template = models.QuestionTemplate()

    new_template.templateId = template.templateId
    new_template.title = template.title
    new_template.description = template.description
    new_template.createdAt = template.createdAt

    try:
        # start a transaction so that we can rollback if an error occurs
        async with db.begin():

            db.add(instance=new_template)
            await db.flush()

        # Create the questions for the template
        new_questions: List[models.Question] = []
        for index, question in enumerate(iterable=template.questions):
            new_question = models.Question(
                templateId=new_template.templateId,
                questionId=index,
                questionText=question.questionText,
                questionType=question.questionType,
                questionOptions=question.questionOptions,
            )
            new_questions.append(new_question)

    except Exception as e:
        # rollback on error
        await db.rollback()
        raise e

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
