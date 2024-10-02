from fastapi import Request, APIRouter, HTTPException
from typing import List, Literal

from backend.lib.api.logs import logger
from backend.lib.api.logs.utils import read_logs, get_log_file_names_on_disk
from backend.lib.api.logs.models import LogEntry

router = APIRouter()


@router.get(path="/logs/get", tags=["logs"], response_model=List[LogEntry])
def get_logs(
    request: Request,
    log_name: str,
    start_line: int = 0,
    amount: int = 100,
    log_severity: Literal["DEBUG", "INFO", "WARNING", "ERROR", "CRITICAL"] = "INFO",
    order: Literal["asc", "desc"] = "asc",
) -> List[LogEntry]:
    try:
        logs: List[LogEntry] = read_logs(
            log_name=log_name,
            start_line=start_line,
            amount=amount,
            log_severity=log_severity,
            order=order,
        )
        return logs
    except FileNotFoundError:
        raise HTTPException(status_code=404, detail="Log file not found")
    except Exception as e:
        logger.error(
            msg="An error occurred while trying to read the logs",
            exc_info=e,
        )
        raise HTTPException(
            status_code=500,
            detail=(
                "An error occurred while trying to read the logs. "
                "Please try again later. If the problem persists, contact the system administrator. "
                "If you are the system administrator, check the logs for more information."
            ),
        )


@router.get(path="/logs/get-available", tags=["logs"], response_model=List[str])
def get_available_logs(
    request: Request,
) -> List[str]:
    return get_log_file_names_on_disk()
