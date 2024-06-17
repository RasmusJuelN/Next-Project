"""
Log Helper Module

Author: Nicklas H. (LobaDK)
Date: 2024

This module is part of a collection of utilities intended to simplify logging in Python applications. It is provided "as is" for anyone to use, modify, and distribute, freely and openly. While not required, credit back to the original author is appreciated.

This module is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
"""

import logging
from logging import StreamHandler
from logging.handlers import TimedRotatingFileHandler
from typing import overload, Literal, Union
from pathlib import Path
from os import makedirs


class LogHelper:
    """
    A convenience class to create a logger with a file handler and a stream handler, or, optionally, a rotating file handler.

    While the class can be instantiated, it is designed to be used as a static class. The `create_logger` method is the primary method to use.
    Calling `create_logger` will automatically create an instance of LogHelper and use its class methods to create the logger.

    If a matching existing logger is found, the method will return the existing logger. Missing log directories will be created.

    Args:
        logger_name (str): The name of the logger.
        log_file (str): The path and name of the log file.
        file_log_level (int): The log level for the file handler.
        stream_log_level (int): The log level for the stream handler.
        rotate_logs (bool): Whether to rotate log files.
        when (str): The interval at which log files should be rotated (e.g., 'midnight', 'daily', 'weekly', 'monthly').
        interval (int): The interval at which log files should be rotated.
        backup_count (int): The number of backup log files to keep.

    Attributes:
        Attributes are the same as the arguments.

    Methods:
        create_logger: Creates a logger with a file handler and a stream handler.
        logger_exists: Checks if a logger with the given name exists.
        create_file_handler: Creates a logging FileHandler with the specified log file and level.
        create_stream_handler: Creates a logging StreamHandler with the specified log level.
        create_timed_rotating_file_handler: Creates a logging TimedRotatingFileHandler with the specified log file, level, interval, and backup count.
        create_log_dir: Creates a log directory if it does not exist.
    """

    def __init__(
        self,
        logger_name: str,
        log_file: str,
        file_log_level: int,
        stream_log_level: int,
        rotate_logs: bool,
        when: str,
        interval: int,
        backup_count: int,
    ) -> None:
        self.logger_name: str = logger_name
        self.log_file: str = log_file
        self.file_log_level: int = file_log_level
        self.stream_log_level: int = stream_log_level
        self.rotate_logs: bool = rotate_logs
        self.when: str = when
        self.interval: int = interval
        self.backup_count: int = backup_count

    @overload
    @staticmethod
    def create_logger(
        logger_name: str,
        log_file: str,
        *,
        file_log_level: int = logging.INFO,
        stream_log_level: int = logging.ERROR,
        rotate_logs: Literal[False] = False,
    ) -> logging.Logger:
        """
        Factory method to create a logger with a file handler and a stream handler, or, optionally, a rotating file handler.

        This factory method makes it convenient to create a logger with a file handler and a stream handler.
        If a logger with the given name already exists, the method will return the existing logger. Missing log directories will be created.

        Args:
            logger_name (str): The name of the logger.
            log_file (str): The path and name of the log file.
            file_log_level (int, optional): The log level for the file handler. Defaults to logging.INFO.
            stream_log_level (int, optional): The log level for the stream handler. Defaults to logging.ERROR.
            rotate_logs (Literal[False], optional): Whether to rotate log files. Defaults to False.

        Returns:
            logging.Logger: The created logger object.
        """
        ...

    @overload
    @staticmethod
    def create_logger(
        logger_name: str,
        log_file: str,
        *,
        file_log_level: int = logging.INFO,
        stream_log_level: int = logging.ERROR,
        rotate_logs: Literal[True] = True,
        when: str = "midnight",
        interval: int = 1,
        backup_count: int = 7,
    ) -> logging.Logger:
        """
        Factory method to create a logger with a rotating file handler and a stream handler.

        This factory method makes it convenient to create a logger with a rotating file handler and a stream handler.
        If a logger with the given name already exists, the method will return the existing logger. Missing log directories will be created.

        Args:
            logger_name (str): The name of the logger.
            log_file (str): The path and name of the log file.
            file_log_level (int, optional): The log level for the file handler. Defaults to logging.INFO.
            stream_log_level (int, optional): The log level for the stream handler. Defaults to logging.ERROR.
            rotate_logs (Literal[True]): Whether to rotate log files.
            when (str, optional): The interval at which log files should be rotated (e.g., 'midnight', 'daily', 'weekly', 'monthly'). Defaults to "midnight".
            interval (int, optional): The interval at which log files should be rotated. Defaults to 1.
            backup_count (int, optional): The number of backup log files to keep. Defaults to 7.

        Returns:
            logging.Logger: The created logger object.
        """
        ...

    @staticmethod
    def create_logger(
        logger_name: str,
        log_file: str,
        *,
        file_log_level: int = logging.INFO,
        stream_log_level: int = logging.ERROR,
        rotate_logs: bool = False,
        when: str = "midnight",
        interval: int = 1,
        backup_count: int = 7,
    ) -> logging.Logger:
        log_creator = LogHelper(
            logger_name=logger_name,
            log_file=log_file,
            file_log_level=file_log_level,
            stream_log_level=stream_log_level,
            rotate_logs=rotate_logs,
            when=when,
            interval=interval,
            backup_count=backup_count,
        )
        if not Path(log_file).parent.exists():
            log_creator.create_log_dir(log_dir=Path(log_file).parent.as_posix())

        if log_creator.logger_exists():
            return logging.getLogger(name=logger_name)

        logger: logging.Logger = logging.getLogger(name=logger_name)
        logger.setLevel(
            level=logging.DEBUG
        )  # The minimum level of the logger itself. Handlers can have different levels, but if they're lower than this, they won't log anything.

        stream_handler: StreamHandler = log_creator.create_stream_handler()
        logger.addHandler(hdlr=stream_handler)

        file_handler: Union[logging.FileHandler, TimedRotatingFileHandler]
        if rotate_logs:
            file_handler = log_creator.create_timed_rotating_file_handler()
        else:
            file_handler = log_creator.create_file_handler()

        logger.addHandler(hdlr=file_handler)

        return logger

    def logger_exists(self) -> bool:
        """
        Checks if a logger with the given name exists.

        Args:
            logger_name (str): The name of the logger to check.

        Returns:
            bool: True if a logger with the given name exists, False otherwise.
        """
        return self.logger_name in logging.Logger.manager.loggerDict

    def create_file_handler(self) -> logging.FileHandler:
        """
        Creates a logging FileHandler with the specified log file and level.

        Args:
            log_file (str): The path and name of the log file to create.

        Returns:
            logging.FileHandler: The file handler object.

        """
        handler = logging.FileHandler(
            filename=self.log_file,
            encoding="utf-8",
            mode="a",
        )
        date_format = "%Y-%m-%d %H:%M:%S"
        formatter = logging.Formatter(
            fmt="[{asctime}] [{levelname:<8}] {name}: {message}",
            datefmt=date_format,
            style="{",
        )
        handler.setFormatter(fmt=formatter)
        handler.setLevel(level=self.file_log_level)
        return handler

    def create_stream_handler(self) -> logging.StreamHandler:
        """
        Creates a logging StreamHandler with the specified log level.

        Returns:
            logging.StreamHandler: The created StreamHandler object.

        """
        handler = logging.StreamHandler()
        date_format = "%Y-%m-%d %H:%M:%S"
        formatter = logging.Formatter(
            fmt="[{asctime}] [{levelname:<8}] {name}: {message}",
            datefmt=date_format,
            style="{",
        )
        handler.setFormatter(fmt=formatter)
        handler.setLevel(level=self.stream_log_level)
        return handler

    def create_timed_rotating_file_handler(self) -> TimedRotatingFileHandler:
        """
        Creates a logging TimedRotatingFileHandler with the specified log file, level, interval, and backup count.

        Args:
            log_file (str): The path and name of the log file to create.
            level (int): The log level for the file handler.
            interval (str): The interval at which log files should be rotated (e.g., 'midnight', 'daily', 'weekly', 'monthly').
            backup_count (int): The number of backup log files to keep.

        Returns:
            logging.handlers.TimedRotatingFileHandler: The created TimedRotatingFileHandler object.
        """
        handler = TimedRotatingFileHandler(
            filename=self.log_file,
            when=self.when,
            interval=self.interval,
            backupCount=self.backup_count,
            encoding="utf-8",
        )
        date_format = "%Y-%m-%d %H:%M:%S"
        formatter = logging.Formatter(
            fmt="[{asctime}] [{levelname:<8}] {name}: {message}",
            datefmt=date_format,
            style="{",
        )
        handler.setFormatter(fmt=formatter)
        handler.setLevel(level=self.file_log_level)
        return handler

    def create_log_dir(self, log_dir: str) -> None:
        """
        Creates a log directory if it does not exist.

        Args:
            log_dir (str): The path to the log directory.

        """
        makedirs(name=log_dir, exist_ok=True)
