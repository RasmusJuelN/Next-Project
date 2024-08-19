from backend.lib.settings.settings_manager import SettingsManagerWithDataclass
from backend.lib.settings.models import AppSettings, app_settings_as_dataclass


app_settings: SettingsManagerWithDataclass[AppSettings] = SettingsManagerWithDataclass[
    AppSettings
](
    path="./backend-config.yaml",
    default_settings=app_settings_as_dataclass,
    autosave=True,
)
