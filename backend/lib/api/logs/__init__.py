from logging import Logger, DEBUG, INFO


from backend.lib._logger import LogHelper

logger: Logger = LogHelper.create_logger(
    logger_name="backend API (logs)",
    log_file="backend/logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)
