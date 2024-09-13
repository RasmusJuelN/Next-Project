from urllib import parse
from typing import Dict, Any, Literal

from backend import app_settings


def build_sqlite_connection_args() -> Dict[str, Any]:
    # dialect+driver:///<database path and name>
    dialect = "sqlite"
    driver: str = app_settings.settings.database.database_driver or "aiosqlite"
    db_name: str = app_settings.settings.database.db_name
    return {
        "url": f"{dialect}+{driver}:///{db_name}",
    }


def build_mysql_connection_args() -> Dict[str, Any]:
    # dialect+driver://<username>:<password>@<host>:<port>/<database>
    dialect = "mysql"
    driver: str = app_settings.settings.database.database_driver or "aiomysql"
    user: str = app_settings.settings.database.user or "root"
    password: str = parse.quote_plus(
        string=app_settings.settings.database.password or ""
    )
    host: str = app_settings.settings.database.host or "localhost"
    port: int = app_settings.settings.database.port or 3306
    db_name: str = app_settings.settings.database.db_name

    if app_settings.settings.database.use_ssl:
        connect_args: Dict[str, Any] = {
            "ssl": {
                "cert": app_settings.settings.database.ssl_cert_file,
                "key": app_settings.settings.database.ssl_key_file,
                "ca": app_settings.settings.database.ssl_ca_cert_file,
            }
        }
    else:
        connect_args = {}

    return {
        "url": f"{dialect}+{driver}://{user}:{password}@{host}:{port}/{db_name}",
        "connect_args": connect_args,
    }


def build_mssql_connection_args() -> Dict[str, Any]:
    # dialect+driver://<username>:<password>@<dsn_name>
    dialect = "mssql"
    driver: str = app_settings.settings.database.database_driver or "aioodbc"
    user: str = app_settings.settings.database.user or "sa"
    password: str = parse.quote_plus(
        string=app_settings.settings.database.password or ""
    )
    host: str = app_settings.settings.database.host or "localhost"
    port: int = app_settings.settings.database.port or 1433
    db_name: str = app_settings.settings.database.db_name

    return {
        "url": f"{dialect}+{driver}://{user}:{password}@{host}:{port}/{db_name}",
    }


def build_db_connection_args() -> Dict[str, Any]:
    database_type: Literal["sqlite"] | Literal["mysql"] | Literal["mssql"] = (
        app_settings.settings.database.database_type
    )
    if database_type == "sqlite":
        return build_sqlite_connection_args()
    elif database_type == "mysql":
        return build_mysql_connection_args()
    elif database_type == "mssql":
        return build_mssql_connection_args()
    else:
        raise ValueError(f"Unsupported database type: {database_type}")
