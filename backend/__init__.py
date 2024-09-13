from logging import Logger, DEBUG, CRITICAL

from backend.lib.settings.settings_manager import SettingsManagerWithDataclass
from backend.lib.settings.models import AppSettings
from backend.lib._logger import LogHelper

logger: Logger = LogHelper.create_logger(
    logger_name="backend.lib.settings",
    log_file="backend/logs/settings_manager.log",
    file_log_level=DEBUG,
    stream_log_level=CRITICAL,
    ignore_existing=True,
)

app_settings: SettingsManagerWithDataclass[AppSettings] = SettingsManagerWithDataclass[
    AppSettings
](
    path="./backend-config.yaml",
    default_settings=AppSettings(),
    autosave=True,
)
