from ..settings.settings import app_settings

SECRET_KEY: str = app_settings["AUTH"][
    "secret_key"
]  # TODO: CHANGE BEFORE DEPLOYMENT AND MOVE TO ENVIRONMENT VARIABLES
ALGORITHM: str = app_settings["AUTH"]["algorithm"]
ACCESS_TOKEN_EXPIRE_MINUTES: int = app_settings["AUTH"]["access_token_expire_minutes"]

DOMAIN: str = app_settings["AUTH"]["domain"]
LDAP_SERVER: str = f"ldap://{DOMAIN}"
LDAP_BASE_DN: str = app_settings["AUTH"]["ldap_base_dn"]

SCOPES: dict[str, str] = app_settings["AUTH"]["scopes"]

app_settings.save()
