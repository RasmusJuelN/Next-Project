from fastapi import Request, APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from typing import Sequence, Optional

from backend.lib.sql.dependencies import get_db
from backend.lib.sql import crud, schemas, models
from backend.lib.api.questionnaire.models import (
    TemplateSearchRequest,
)
from backend.lib.api.questionnaire.utility import query_templates, query_template_by_id

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
    return query_templates(query=query, db=db)


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


@router.post(
    path="/questionnaire/create",
    tags=["questionnaire"],
    response_model=schemas.ActiveQuestionnaireModel,
)
def create_active_questionnaire(
    request: Request,
    questionnaire: schemas.ActiveQuestionnaireCreateModel,
    db: Session = Depends(dependency=get_db),
) -> schemas.ActiveQuestionnaireModel:
    new_questionnaire: models.ActiveQuestionnaire = crud.add_active_questionnaire(
        db=db, questionnaire=questionnaire
    )
    new_student = schemas.User(
        id=new_questionnaire.student.id,
        user_name=new_questionnaire.student.username,
        full_name=new_questionnaire.student.full_name,
        role=new_questionnaire.student.role,
    )
    new_teacher = schemas.User(
        id=new_questionnaire.teacher.id,
        user_name=new_questionnaire.teacher.username,
        full_name=new_questionnaire.teacher.full_name,
        role=new_questionnaire.teacher.role,
    )
    template = crud.get_template_by_id(
        db=db, template_id=new_questionnaire.template_reference_id
    )
    if template is None:
        raise HTTPException(status_code=404, detail="Template not found")

    questionnaire_to_return = schemas.ActiveQuestionnaireModel(
        id=new_questionnaire.id,
        student=new_student,
        teacher=new_teacher,
        is_student_finished=new_questionnaire.is_student_finished,
        is_teacher_finished=new_questionnaire.is_teacher_finished,
        questionnaire_template=schemas.QuestionnaireTemplateModel(
            template_id=template.template_id,
            title=template.title,
            description=template.description,
        ),
        created_at=new_questionnaire.created_at,
    )

    return questionnaire_to_return
