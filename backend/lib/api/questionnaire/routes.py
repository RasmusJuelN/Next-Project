from fastapi import Request, APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from typing import Sequence, Optional, Annotated

from backend.lib.sql.dependencies import get_db
from backend.lib.sql import crud, schemas, models
from backend.lib.api.questionnaire.models import (
    TemplateSearchRequest,
    QuestionnaireSearchRequest,
)
from backend.lib.api.questionnaire.utility import (
    query_templates,
    query_template_by_id,
    query_questionnaires,
)

router = APIRouter()


# You may notice that the response_model and return type does not match.
# Pydantic, which is used to define the response_model, automatically
# converts the return type to the response_model if 'from_attributes'
# (formerly 'orm_mode') is set to True in the model's Config, While the
# return type is an SQLAlchemy model, allowing internal code to still
# correctly infer the type of data being returned. Additionally, if
# 'from_attributes' is True, pydantic will be able to get the attributes
# of any lazy-loading relationships that are accessed in the SQLAlchemy model.
@router.post(
    path="/templates/create",
    tags=["template"],
    response_model=schemas.CreateQuestionTemplateModel,
)
def create_template(
    request: Request,
    template: schemas.CreateQuestionTemplateModel,
    db: Annotated[Session, Depends(dependency=get_db)],
) -> models.QuestionTemplate:
    return crud.add_template(db=db, template=template)


@router.get(
    path="/templates/query",
    tags=["template"],
    response_model=Sequence[schemas.QuestionTemplateModel],
)
def get_templates(
    request: Request,
    query: Annotated[TemplateSearchRequest, Depends()],
    db: Annotated[Session, Depends(dependency=get_db)],
) -> Sequence[models.QuestionTemplate]:
    return query_templates(query=query, db=db)


@router.get(
    path="/templates/get/{template_id}",
    tags=["template"],
    response_model=schemas.QuestionTemplateModel,
)
def get_template(
    request: Request,
    template_id: str,  # The ID of the template the client wishes to fetch
    db: Annotated[Session, Depends(dependency=get_db)],
) -> models.QuestionTemplate:
    template: Optional[models.QuestionTemplate] = query_template_by_id(
        template_id=template_id, db=db
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
    db: Annotated[Session, Depends(dependency=get_db)],
) -> models.QuestionTemplate:
    return crud.update_template(
        db=db, existing_id=template_id, updated_template=template
    )


@router.delete(
    path="/templates/delete/{template_id}",
    tags=["template"],
    response_model=schemas.QuestionTemplateModel,
)
def delete_template(
    request: Request,
    template_id: str,  # The ID of the template the client wishes to delete
    db: Annotated[Session, Depends(dependency=get_db)],
) -> models.QuestionTemplate:
    return crud.delete_template(db=db, template=template_id)


@router.post(
    path="/questionnaire/create",
    tags=["questionnaire"],
    response_model=schemas.ActiveQuestionnaireModel,
)
def create_questionnaire(
    request: Request,
    questionnaire: schemas.ActiveQuestionnaireCreateModel,
    db: Annotated[Session, Depends(dependency=get_db)],
) -> models.ActiveQuestionnaire:
    new_questionnaire: models.ActiveQuestionnaire = crud.add_active_questionnaire(
        db=db, questionnaire=questionnaire
    )
    return new_questionnaire


@router.get(
    path="/questionnaire/query",
    tags=["questionnaire"],
    response_model=Sequence[schemas.ActiveQuestionnaireModel],
)
def get_questionnaires(
    request: Request,
    query: Annotated[QuestionnaireSearchRequest, Depends()],
    db: Annotated[Session, Depends(dependency=get_db)],
) -> Sequence[models.ActiveQuestionnaire]:
    return query_questionnaires(query=query, db=db)


@router.get(
    path="/questionnaire/active/{questionnaire_id}",
    tags=["questionnaire"],
    response_model=schemas.ActiveQuestionnaireModel,
)
def get_active_questionnaire(
    request: Request,
    questionnaire_id: str,
    db: Annotated[Session, Depends(dependency=get_db)],
) -> models.ActiveQuestionnaire:
    questionnaire: Optional[models.ActiveQuestionnaire] = (
        crud.get_active_questionnaire_by_id(db=db, questionnaire_id=questionnaire_id)
    )
    if questionnaire is None:
        raise HTTPException(status_code=404, detail="Questionnaire not found")
    return questionnaire


@router.get(
    path="/questionnaire/active/check/{user_id}",
    tags=["questionnaire"],
    response_model=Optional[str],
)
def check_if_user_has_active_questionnaires(
    request: Request,
    user_id: str,
    db: Annotated[Session, Depends(dependency=get_db)],
) -> Optional[str]:
    return crud.get_oldest_active_questionnaire_id_for_user(db=db, user_id=user_id)


@router.get(
    path="/questionnaire/active/check-all/{user_id}",
    tags=["questionnaire"],
    response_model=Sequence[str],
)
def check_all_active_questionnaires_for_user(
    request: Request,
    user_id: str,
    db: Annotated[Session, Depends(dependency=get_db)],
) -> Sequence[str]:
    return crud.get_all_active_questionnaire_ids_for_user(db=db, user_id=user_id)
