from typing import Tuple, Optional
from sqlalchemy import Result, select
from sqlalchemy.ext.asyncio import AsyncSession

from backend.lib.sql import models, schemas
from backend.lib.sql.exceptions import TemplateNotFoundException


async def get_template_by_id(
    db: AsyncSession, template: schemas.QuestionTemplateBase
) -> Optional[models.QuestionTemplate]:
    """
    Retrieve a question template by its ID from the database.

    Args:
        db (AsyncSession): The database session to use for the query.
        template (schemas.QuestionTemplateBase): The template schema containing the ID of the template to retrieve.

    Returns:
        Optional[models.QuestionTemplate]: The question template if found, otherwise None.
    """
    await db.flush()
    result: Result[Tuple[models.QuestionTemplate]] = await db.execute(
        statement=select(models.QuestionTemplate).where(
            models.QuestionTemplate.template_id == template.template_id
        )
    )
    return result.scalars().first()


async def add_template(
    db: AsyncSession, template: schemas.QuestionTemplateCreate
) -> models.QuestionTemplate:
    """
    Asynchronously adds a new question template to the database.

    Args:
        db (AsyncSession): The database session to use for the operation.
        template (schemas.QuestionTemplateCreate): The template data to be added.

    Returns:
        models.QuestionTemplate: The newly created question template instance.
    """
    new_template = models.QuestionTemplate(
        template_id=template.template_id,
        title=template.title,
        description=template.description,
        created_at=template.created_at,
    )
    db.add(instance=new_template)
    await db.flush()
    await db.commit()
    return new_template


async def update_template(
    db: AsyncSession, template: schemas.QuestionTemplateUpdate
) -> models.QuestionTemplate:
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
    updated_template: Optional[models.QuestionTemplate] = await get_template_by_id(
        db=db, template=template
    )
    if not updated_template:
        raise TemplateNotFoundException(template_id=template.template_id)

    updated_template.title = template.title
    updated_template.description = template.description
    updated_template.createdAt = template.created_at
    await db.commit()
    await db.flush()
    return updated_template


async def delete_template(
    db: AsyncSession, template: schemas.QuestionTemplateBase
) -> models.QuestionTemplate:
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
    deleted_template: Optional[models.QuestionTemplate] = await get_template_by_id(
        db=db, template=template
    )
    if not deleted_template:
        raise TemplateNotFoundException(template_id=template.template_id)

    await db.delete(instance=deleted_template)
    await db.commit()
    return deleted_template
