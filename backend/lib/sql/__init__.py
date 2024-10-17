from logging import Logger, DEBUG, WARNING

from backend.lib._logger import LogHelper

db_logger: Logger = LogHelper.create_logger(
    logger_name="sqlalchemy",
    log_file="backend/logs/sqlalchemy.log",
    file_log_level=DEBUG,
    stream_log_level=WARNING,
    ignore_existing=True,
)

database_migration_logger: Logger = LogHelper.create_logger(
    logger_name="alembic.runtime.migration",
    log_file="backend/logs/alembic.log",
    file_log_level=DEBUG,
    stream_log_level=WARNING,
    ignore_existing=True,
)
