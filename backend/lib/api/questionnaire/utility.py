from sqlalchemy.orm import Session
from typing import Sequence, cast, Optional

from backend.lib import cache
from backend.lib.sql import crud, models
from backend.lib.api.questionnaire.models import (
    TemplateSearchRequest,
    QuestionnaireSearchRequest,
)


def query_templates(
    query: TemplateSearchRequest, db: Session
) -> Sequence[models.QuestionTemplate]:
    """
    Retrieve a paginated list of question templates based on the search query.

    This function first attempts to read the templates from the cache using a
    generated cache key. If the templates are found in the cache, it returns
    the paginated result. If not, it queries the database for the templates,
    caches the result, and then returns the paginated result.

    Args:
        query (TemplateSearchRequest): The search query containing pagination
            information and an optional title to filter the templates.
        db (Session): The database session used to query the templates.

    Returns:
        Sequence[models.QuestionTemplate]: A list of question templates
        matching the search query, limited by pagination parameters.
    """
    start: int = (query.page - 1) * query.limit

    cached_key: str = (
        f"query_templates_{query.title}" if query.title else "query_templates"
    )

    cached_templates: Optional[Sequence[models.QuestionTemplate]] = cast(
        Optional[Sequence[models.QuestionTemplate]], cache.read(key=cached_key)
    )

    if cached_templates:
        return cached_templates[start : start + query.limit]  # noqa: E203

    if query.title is None:
        templates: Sequence[models.QuestionTemplate] = crud.get_all_templates(db=db)
    else:
        templates = crud.get_templates_by_title(db=db, title=query.title)

    cache.write(key=cached_key, value=templates)

    return templates[start : start + query.limit]  # noqa: E203


def query_template_by_id(
    template_id: str, db: Session
) -> Optional[models.QuestionTemplate]:
    """
    Retrieve a question template by its ID, utilizing caching for performance.

    Args:
        template_id (str): The ID of the question template to retrieve.
        db (Session): The database session to use for querying.

    Returns:
        Optional[models.QuestionTemplate]: The retrieved question template, or None if not found.

    Raises:
        HTTPException: If the template is not found in the database.
    """
    cached_key: str = f"query_template_by_id_{template_id}"

    cached_template: Optional[models.QuestionTemplate] = cast(
        Optional[models.QuestionTemplate], cache.read(key=cached_key)
    )

    if cached_template:
        return cached_template

    template: Optional[models.QuestionTemplate] = crud.get_template_by_id(
        db=db, template_id=template_id
    )

    if template is not None:
        cache.write(key=cached_key, value=template)

    return template


def query_questionnaires(
    query: QuestionnaireSearchRequest, db: Session
) -> Sequence[models.ActiveQuestionnaire]:
    start: int = (query.page - 1) * query.limit

    cached_key: str = (
        f"query_questionnaires_{query.search_student}_{query.search_teacher}"
    )

    cached_questionnaires: Optional[Sequence[models.ActiveQuestionnaire]] = cast(
        Optional[Sequence[models.ActiveQuestionnaire]], cache.read(key=cached_key)
    )

    if cached_questionnaires:
        return cached_questionnaires[start : start + query.limit]  # noqa: E203

    questionnaires: Sequence[models.ActiveQuestionnaire] = (
        crud.get_all_active_questionnaires(
            db=db, teacher=query.search_teacher, student=query.search_student
        )
    )

    cache.write(key=cached_key, value=questionnaires)

    return questionnaires[start : start + query.limit]  # noqa: E203
