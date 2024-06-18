from .settings_manager import SettingsManager
from .models import user_settings, UserSettingsObject


app_settings = SettingsManager(
    "./backend-config.yaml",
    default_settings=user_settings,
    use_logger=True,
    log_file="./backend/logs/settings.log",
    save_on_exit=True,
)

app_settings_object: UserSettingsObject = app_settings.to_object(
    data_class=UserSettingsObject
)
