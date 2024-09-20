from fastapi import Request, APIRouter
from logging import Logger, DEBUG, INFO
from typing import List, Optional

from backend.lib._logger import LogHelper

logger: Logger = LogHelper.create_logger(
    logger_name="backend API (logs)",
    log_file="backend/logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)

router = APIRouter()


@router.get(path="/logs/get", tags=["logs"], response_model=List[str])
def get_logs(
    request: Request,
    log_name: str,
    start_line: Optional[int] = 0,
    amount: Optional[int] = 100,
    log_severity: Optional[str] = "INFO",
    order: Optional[str] = "asc",
) -> List[str]:
    raise NotImplementedError
