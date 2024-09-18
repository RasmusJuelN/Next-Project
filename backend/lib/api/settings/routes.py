from fastapi import Request, APIRouter
from logging import Logger, DEBUG, INFO

from backend import AppSettings, app_settings

from backend.lib._logger import LogHelper


logger: Logger = LogHelper.create_logger(
    logger_name="backend API (settings)",
    log_file="backend/logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)

router = APIRouter()


@router.get(
    path="/settings/get",
    tags=["settings"],
    response_model=AppSettings,
)
async def get_settings(
    request: Request,
) -> AppSettings:
    return app_settings.settings


@router.put(
    path="/settings/update",
    tags=["settings"],
    response_model=AppSettings,
)
async def update_settings(
    request: Request,
    settings: AppSettings,
) -> AppSettings:
    app_settings.settings = settings
    return app_settings.settings
