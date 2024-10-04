from dataclasses import field
from pydantic.dataclasses import dataclass
from pydantic import AliasGenerator, ConfigDict, BaseModel, Field
from pydantic.alias_generators import to_camel
from typing import Optional, Literal, Sequence, Union, List


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
    salt_hash: Optional[str] = field(default=None)
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
    authentication_method: Literal["simple", "sasl-digest-md5", "NTLM"] = field(
        default="simple"
    )


@dataclass(config=config)
class AppSettings:
    auth: AuthSettings = field(default_factory=AuthSettings)
    database: DatabaseSettings = field(default_factory=DatabaseSettings)


class CamelCaseModel(BaseModel):
    class Config:
        alias_generator = AliasGenerator(
            validation_alias=to_camel,
            serialization_alias=to_camel,
        )
        populate_by_name = True


class Metadata(CamelCaseModel):
    default: Optional[Union[str, int, bool, Sequence]] = Field(default=None)
    min_value: Optional[int] = Field(default=None)
    max_value: Optional[int] = Field(default=None)
    allowed_values: Optional[List[Union[str, int]]] = Field(default=None)
    can_be_empty: bool = Field(default=False)
    description: Optional[str] = Field(default=None)


class DatabaseSettingsModel(CamelCaseModel):
    database_type: Metadata = Metadata(
        default="sqlite",
        allowed_values=["sqlite", "mysql", "mssql"],
        can_be_empty=False,
        description="The type of database to use.",
    )
