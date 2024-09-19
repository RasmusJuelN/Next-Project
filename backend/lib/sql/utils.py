from urllib import parse
from typing import Dict, Any, Literal, LiteralString
from random import choice
from string import ascii_letters, digits
from sqlalchemy.orm import Session

from backend import app_settings
from backend.lib.sql import models

URL_FRIENDLY_BASE64: LiteralString = ascii_letters + digits + "-_"


def build_sqlite_connection_args() -> Dict[str, Any]:
    # dialect+driver:///<database path and name>
    dialect = "sqlite"
    driver: str = app_settings.settings.database.database_driver or "pysqlite"
    db_name: str = app_settings.settings.database.db_name
    return {
        "url": f"{dialect}+{driver}:///{db_name}",
    }


def build_mysql_connection_args() -> Dict[str, Any]:
    # dialect+driver://<username>:<password>@<host>:<port>/<database>
    dialect = "mysql"
    driver: str = app_settings.settings.database.database_driver or "pymysql"
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
    driver: str = app_settings.settings.database.database_driver or "pyodbc"
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


def generate_random_string(
    length: int = 10, char_set: str = URL_FRIENDLY_BASE64
) -> str:
    """
    Generate a random string of a specified length from a given character set.

    Args:
        length (int): The length of the random string to generate. Default is 10.
        char_set (str): The set of characters to use for generating the random string. Default is URL_FRIENDLY_BASE64.

    Returns:
        str: A randomly generated string of the specified length.

    Note:
        `URL_FRIENDLY_BASE64` is ascii_letters + digits + "-_". Ascii_letters is A-Z and a-z.
    """
    return "".join(choice(seq=char_set) for _ in range(length))


def id_exists(db: Session, id: str) -> bool:
    """
    Check if a given template ID exists in the database.

    Args:
        db: The database session.
        id (str): The template ID to check for in the database.

    Returns:
        bool: True if the template ID exists in the database, False otherwise.
    """
    return (
        db.query(models.QuestionTemplate).filter_by(template_id=id).first() is not None
    )
