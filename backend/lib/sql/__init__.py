from logging import Logger, DEBUG, CRITICAL

from backend.lib._logger import LogHelper

db_logger: Logger = LogHelper.create_logger(
    logger_name="sqlalchemy",
    log_file="backend/logs/sql.log",
    file_log_level=DEBUG,
    stream_log_level=CRITICAL,
    ignore_existing=True,
)
