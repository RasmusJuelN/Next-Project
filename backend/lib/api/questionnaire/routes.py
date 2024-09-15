from fastapi import Request, APIRouter, Depends
from logging import Logger, DEBUG, INFO
from sqlalchemy.ext.asyncio import AsyncSession

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


@router.post(path="/templates/create", tags=["questionnaire"], response_model=schemas.QuestionTemplateCreate)
async def create_template(
    request: Request,
    template: schemas.QuestionTemplateCreate,
    db: AsyncSession = Depends(dependency=get_db),
) -> schemas.QuestionTemplateCreate:
    return await crud.add_template(db=db, template=template)
