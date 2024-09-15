from backend.lib.settings.settings_manager import SettingsManagerWithDataclass
from backend.lib.settings.models import AppSettings


app_settings: SettingsManagerWithDataclass[AppSettings] = SettingsManagerWithDataclass[
    AppSettings
](
    path="./backend-config.yaml",
    default_settings=AppSettings(),
    autosave=True,
)
