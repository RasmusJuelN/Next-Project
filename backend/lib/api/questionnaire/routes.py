from fastapi import Request, APIRouter, Depends
from logging import Logger, DEBUG, INFO
from sqlalchemy.orm import Session
from typing import Sequence

from backend.lib._logger import LogHelper
from backend.lib.sql.dependencies import get_db
from backend.lib.sql import crud, schemas

logger: Logger = LogHelper.create_logger(
    logger_name="backend API (questionnaire)",
    log_file="backend/logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)

router = APIRouter()


@router.post(
    path="/templates/create",
    tags=["questionnaire"],
    response_model=schemas.QuestionTemplateCreate,
)
def create_template(
    request: Request,
    template: schemas.QuestionTemplateCreate,
    db: Session = Depends(dependency=get_db),
) -> schemas.QuestionTemplate:
    return crud.add_template(db=db, template=template)


@router.get(
    path="/templates",
    tags=["questionnaire"],
    response_model=Sequence[schemas.QuestionTemplate],
)
def get_templates(
    request: Request, db: Session = Depends(dependency=get_db)
) -> Sequence[schemas.QuestionTemplate]:
    return crud.get_templates(db=db)


@router.put(
    path="/templates/update/{template_id}",
    tags=["questionnaire"],
    response_model=schemas.QuestionTemplate,
)
def update_template(
    request: Request,
    template_id: str,  # The ID of the template the client wishes to update
    template: schemas.QuestionTemplateUpdate,  # The updated template data
    db: Session = Depends(dependency=get_db),
) -> schemas.QuestionTemplate:
    return crud.update_template(
        db=db, existing_template_id=template_id, updated_question_template=template
    )
