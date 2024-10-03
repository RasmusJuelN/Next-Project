from fastapi import Request, APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from typing import Sequence, Optional

from backend.lib.sql.dependencies import get_db
from backend.lib.sql import crud, schemas, models
from backend.lib.api.questionnaire.models import (
    TemplateSearchRequest,
)

router = APIRouter()


# You may notice that the response_model and return type does not match.
# Pydantic, which is used to define the response_model, automatically converts
#   the return type to the response_model if 'from_attributes' (formerly 'orm_mode') is set to True in the model's Config.
# The return type is an SQLAlchemy model which correctly lets internal calling code know what to expect.
@router.post(
    path="/templates/create",
    tags=["template"],
    response_model=schemas.CreateQuestionTemplateModel,
)
def create_template(
    request: Request,
    template: schemas.CreateQuestionTemplateModel,
    db: Session = Depends(dependency=get_db),
) -> models.QuestionTemplate:
    return crud.add_template(db=db, template=template)


@router.get(
    path="/templates/query",
    tags=["template"],
    response_model=Sequence[schemas.QuestionTemplateModel],
)
def get_templates(
    request: Request,
    query: TemplateSearchRequest = Depends(),
    db: Session = Depends(dependency=get_db),
) -> Sequence[models.QuestionTemplate]:
    start: int = (query.page - 1) * query.limit
    if query.title is None:
        templates: Sequence[models.QuestionTemplate] = crud.get_all_templates(db=db)
    else:
        templates = crud.get_templates_by_title(db=db, title=query.title)

    return templates[start : start + query.limit]  # noqa: E203


@router.get(
    path="/templates/get/{template_id}",
    tags=["template"],
    response_model=schemas.QuestionTemplateModel,
)
def get_template(
    request: Request,
    template_id: str,  # The ID of the template the client wishes to fetch
    db: Session = Depends(dependency=get_db),
) -> models.QuestionTemplate:
    template: Optional[models.QuestionTemplate] = crud.get_template_by_id(
        db=db, template_id=template_id
    )
    if template is None:
        raise HTTPException(status_code=404, detail="Template not found")
    return template


@router.put(
    path="/templates/update/{template_id}",
    tags=["template"],
    response_model=schemas.QuestionTemplateModel,
)
def update_template(
    request: Request,
    template_id: str,  # The ID of the template the client wishes to update
    template: schemas.UpdateQuestionTemplateModel,  # The updated template data
    db: Session = Depends(dependency=get_db),
) -> models.QuestionTemplate:
    return crud.update_template(
        db=db, existing_template_id=template_id, updated_template=template
    )


@router.delete(
    path="/templates/delete/{template_id}",
    tags=["template"],
    response_model=schemas.QuestionTemplateModel,
)
def delete_template(
    request: Request,
    template_id: str,  # The ID of the template the client wishes to delete
    db: Session = Depends(dependency=get_db),
) -> models.QuestionTemplate:
    return crud.delete_template(db=db, template=template_id)


@router.get(
    path="/questionnaire",
    tags=["questionnaire"],
    response_model=Sequence[schemas.QuestionTemplateModel],
)
def get_active_questionnaires(
    request: Request,
    page: int,
    limit: int,
    search_student: Optional[str] = None,
    search_teacher: Optional[str] = None,
    db: Session = Depends(dependency=get_db),
) -> Sequence[models.QuestionTemplate]:
    start: int = (page - 1) * limit

    raise NotImplementedError("This endpoint is not yet implemented")
