from dataclasses import field
from pydantic.dataclasses import dataclass
from pydantic import AliasGenerator, ConfigDict
from pydantic.alias_generators import to_camel
from typing import Optional, Literal


config = ConfigDict(
    alias_generator=AliasGenerator(
        validation_alias=to_camel,
        serialization_alias=to_camel,
    ),
    populate_by_name=True,
)


@dataclass(config=config)
class DatabaseSettings:
    database_type: Literal["sqlite", "mysql", "mssql"] = field(default="sqlite")
    database_driver: Optional[str] = field(default=None)
    db_name: str = field(default="program.db")
    host: Optional[str] = field(default=None)
    user: Optional[str] = field(default=None)
    password: Optional[str] = field(default=None)
    port: Optional[int] = field(default=None)
    timeout: Optional[int] = field(default=None)
    use_ssl: bool = field(default=False)
    ssl_cert_file: Optional[str] = field(default=None)
    ssl_key_file: Optional[str] = field(default=None)
    ssl_ca_cert_file: Optional[str] = field(default=None)
    max_connections: Optional[int] = field(default=None)
    min_connections: Optional[int] = field(default=None)


@dataclass(config=config)
class AuthSettings:
    secret_key: Optional[str] = field(default=None)
    algorithm: str = field(default="HS256")
    access_token_expire_minutes: int = field(default=30)
    domain: str = field(default="localhost")
    ldap_server: str = field(default="ldap://localhost")
    ldap_base_dn: str = field(default="dc=example,dc=com")
    scopes: dict[str, str] = field(
        default_factory=lambda: {
            "student": "student",
            "teacher": "teacher",
            "admin": "admin",
        }
    )
    ad_service_account: Optional[str] = field(default=None)
    ad_service_password: Optional[str] = field(default=None)


@dataclass(config=config)
class AppSettings:
    auth: AuthSettings = field(default_factory=AuthSettings)
    database: DatabaseSettings = field(default_factory=DatabaseSettings)
