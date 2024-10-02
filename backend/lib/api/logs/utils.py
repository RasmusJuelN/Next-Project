from typing import List, Literal, Optional, cast, Union
from logging import DEBUG, INFO, WARNING, ERROR, CRITICAL
from re import Match, search
from pathlib import Path

from backend.lib.api.logs.models import LogEntry

LOG_LEVEL_STRING_TO_LITERAL: dict[str, int] = {
    "DEBUG": DEBUG,
    "INFO": INFO,
    "WARNING": WARNING,
    "ERROR": ERROR,
    "CRITICAL": CRITICAL,
}


# Captures the timestamp, log level, source and message of a log line into 4 groups, assuming the log line is in the format:
# [2024-09-17 10:44:45] [DEBUG   ] backend.lib.settings: autosave_on_exit is enabled; registering save method.
LOG_CAPTURE_PATTERN: str = (
    r"\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\] \[(\w+)\s*\] ([\w\s\.]+(?: \([\w\s]+\))?): (.+)"
)


def read_logs(
    log_name: str,
    start_line: int,
    amount: int,
    log_severity: Literal["DEBUG", "INFO", "WARNING", "ERROR", "CRITICAL"],
    order: Literal["asc", "desc"],
) -> List[LogEntry]:
    literal_log_severity: int = LOG_LEVEL_STRING_TO_LITERAL[log_severity]
    # [2024-09-17 10:44:45] [DEBUG   ] backend.lib.settings: autosave_on_exit is enabled; registering save method.
    with open(file=f"backend/logs/{log_name}.log", mode="r") as log_file:
        # TODO: Reading the entire file can be resource intensive and even dangerous if the file is too large.
        # However, Python does not have built-in functions for reading a file backwards. Create or find a library that can read a file backwards.
        read_log_lines: List[str] = log_file.readlines()

    if order == "desc":
        read_log_lines.reverse()

    log_lines: List[LogEntry] = []
    # Keep reading until we have the requested amount of log lines
    while len(log_lines) < amount and start_line < len(read_log_lines):
        log_line: str = read_log_lines[start_line]
        # Remove leading and trailing whitespace, newlines, tabs, escaped double quotes and backslashes
        start_line += 1

        if log_line.strip() == "":
            # Skip empty lines
            continue

        log_level: Optional[Match[str]] = search(
            pattern=LOG_CAPTURE_PATTERN,
            string=log_line,
        )
        if log_level is None:
            # Assume it is part of a multiline log message where the previous line was the log level
            last_log_line: LogEntry = log_lines[-1]
            last_log_line.message += log_line
            continue
        else:
            literal_log_level: int = LOG_LEVEL_STRING_TO_LITERAL[log_level.group(2)]
            if literal_log_level <= literal_log_severity:
                timestamp: str = log_level.group(1)
                severity: Union[
                    Literal["DEBUG"],
                    Literal["INFO"],
                    Literal["WARNING"],
                    Literal["ERROR"],
                    Literal["CRITICAL"],
                ] = cast(
                    Literal["DEBUG", "INFO", "WARNING", "ERROR", "CRITICAL"],
                    log_level.group(2),
                )
                source: str = log_level.group(3)
                message: str = log_level.group(4)
                log_lines.append(
                    LogEntry(
                        timestamp=timestamp,
                        severity=severity,
                        source=source,
                        message=message,
                    )
                )

    return log_lines


def get_log_file_names_on_disk() -> List[str]:
    """
    Retrieves the names of all log files in the 'backend/logs' directory.

    Returns:
        List[str]: A list of log file names with a '.log' extension.
    """
    return [
        path.stem
        for path in Path("backend/logs").iterdir()
        if path.is_file() and path.suffix == ".log"
    ]
