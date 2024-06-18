from ..settings.settings import app_settings_object

SECRET_KEY: str = (
    app_settings_object.auth.secret_key
)  # TODO: CHANGE BEFORE DEPLOYMENT AND MOVE TO ENVIRONMENT VARIABLES
ALGORITHM: str = app_settings_object.auth.algorithm
ACCESS_TOKEN_EXPIRE_MINUTES: int = app_settings_object.auth.access_token_expire_minutes

DOMAIN: str = app_settings_object.auth.domain
LDAP_SERVER: str = f"ldap://{DOMAIN}"
LDAP_BASE_DN: str = app_settings_object.auth.ldap_base_dn

SCOPES: dict[str, str] = app_settings_object.auth.scopes
