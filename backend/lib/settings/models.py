from dataclasses import field
from pydantic.dataclasses import dataclass
from pydantic import AliasGenerator, ConfigDict, BaseModel, Field
from pydantic.alias_generators import to_camel
from typing import Optional, Literal, Sequence, Union, List, Dict


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
    default: Optional[Union[str, int, bool, Sequence, Dict]] = Field(default=None)
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
    database_driver: Metadata = Metadata(
        can_be_empty=True,
        description="The driver to use for the database connection.",
    )
    db_name: Metadata = Metadata(
        default="program.db",
        can_be_empty=False,
        description="The name of the database file.",
    )
    host: Metadata = Metadata(
        can_be_empty=True,
        description="The hostname or IP address of the database server.",
    )
    user: Metadata = Metadata(
        can_be_empty=True,
        description="The username to use for the database connection.",
    )
    password: Metadata = Metadata(
        can_be_empty=True,
        description="The password to use for the database connection.",
    )
    port: Metadata = Metadata(
        can_be_empty=True,
        description="The port number to use for the database connection.",
    )
    timeout: Metadata = Metadata(
        can_be_empty=True,
        description="The timeout value for the database connection.",
    )
    use_ssl: Metadata = Metadata(
        default=False,
        description="Whether to use SSL for the database connection.",
    )
    ssl_cert_file: Metadata = Metadata(
        can_be_empty=True,
        description="The path to the SSL certificate file.",
    )
    ssl_key_file: Metadata = Metadata(
        can_be_empty=True,
        description="The path to the SSL key file.",
    )
    ssl_ca_cert_file: Metadata = Metadata(
        can_be_empty=True,
        description="The path to the SSL CA certificate file.",
    )
    max_connections: Metadata = Metadata(
        can_be_empty=True,
        description="The maximum number of connections to allow.",
        min_value=1,
    )
    min_connections: Metadata = Metadata(
        can_be_empty=True,
        description="The minimum number of connections to allow.",
        min_value=1,
    )


class AuthSettingsModel(CamelCaseModel):
    secret_key: Metadata = Metadata(
        can_be_empty=False,
        description="The secret key to use for JWT token generation.",
    )
    salt_hash: Metadata = Metadata(
        can_be_empty=False,
        description="The salt hash to use for hashing various sensitive user data.",
    )
    algorithm: Metadata = Metadata(
        default="HS256",
        allowed_values=["HS256", "HS384", "HS512"],
        can_be_empty=False,
        description="The algorithm to use for token generation.",
    )
    access_token_expire_minutes: Metadata = Metadata(
        default=30,
        description="The number of minutes before an access token expires.",
        min_value=1,
        max_value=1440,
    )
    domain: Metadata = Metadata(
        default="localhost",
        can_be_empty=False,
        description="The domain to display in various locations. Does not have to match the ldap server.",
    )
    ldap_server: Metadata = Metadata(
        default="ldap://localhost",
        can_be_empty=False,
        description="The LDAP server address to use for authentication.",
    )
    ldap_base_dn: Metadata = Metadata(
        default="dc=example,dc=com",
        can_be_empty=False,
        description="The base DN to use for LDAP queries.",
    )
    scopes: Metadata = Metadata(
        default={"student": "student", "teacher": "teacher", "admin": "admin"},
        description="The scopes to use for token generation.",
    )
    ad_service_account: Metadata = Metadata(
        can_be_empty=False,
        description="The service account to use for Active Directory authentication.",
    )
    ad_service_password: Metadata = Metadata(
        can_be_empty=False,
        description="The service account password to use for Active Directory authentication.",
    )
    authentication_method: Metadata = Metadata(
        default="simple",
        allowed_values=["simple", "sasl-digest-md5", "NTLM"],
        can_be_empty=False,
        description="The authentication method to use for LDAP queries.",
    )


class AppSettingsMetadata(CamelCaseModel):
    auth: AuthSettingsModel = AuthSettingsModel()
    database: DatabaseSettingsModel = DatabaseSettingsModel()
