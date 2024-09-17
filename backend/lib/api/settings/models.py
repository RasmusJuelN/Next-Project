from pydantic import BaseModel
from pydantic.alias_generators import to_camel
from typing import Dict


class CamelModel(BaseModel):
    class Config:
        alias_generator = to_camel


class DatabaseSettings(CamelModel):
    database_type: str
    database_driver: str
    db_name: str
    host: str
    user: str
    password: str
    port: int
    timeout: int
    use_ssl: bool
    ssl_cert_file: str
    ssl_key_file: str
    ssl_ca_cert_file: str
    max_connections: int
    min_connections: int


class AuthSettings(CamelModel):
    secret_key: str
    algorithm: str
    access_token_expire_minutes: int
    domain: str
    ldap_server: str
    ldap_base_dn: str
    scopes: Dict[str, str]


class AppSettings(CamelModel):
    auth: AuthSettings
    database: DatabaseSettings
