from .settings_manager import SettingsManager
from .constants import DEFAULT_SETTINGS


app_settings = SettingsManager(
    "./backend-config.yaml",
    default_settings=DEFAULT_SETTINGS,
    use_logger=True,
    log_file="./backend/logs/settings.log",
    save_on_exit=True,
)
