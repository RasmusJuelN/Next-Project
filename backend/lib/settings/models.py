from dataclasses import dataclass, field


@dataclass
class AuthSettingsObject:
    secret_key: str = field(default="CHANGE_ME")
    algorithm: str = field(default="HS256")
    access_token_expire_minutes: int = field(default=30)
    domain: str = field(default="localhost")
    ldap_base_dn: str = field(default="dc=example,dc=com")
    scopes: dict[str, str] = field(
        default_factory=lambda: {
            "student": "student",
            "teacher": "teacher",
            "admin": "admin",
        }
    )


@dataclass
class UserSettingsObject:
    auth: AuthSettingsObject = field(default_factory=AuthSettingsObject)


user_settings = UserSettingsObject()
