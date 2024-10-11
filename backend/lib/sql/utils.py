from urllib import parse
from typing import Dict, Any, Literal, LiteralString, TypeAlias, Union
from string import ascii_letters, digits
from sqlalchemy.orm import Session, DeclarativeBase
from sqlalchemy.orm.decl_api import DeclarativeAttributeIntercept
from sqlalchemy.sql import exists, select, or_, ColumnElement

from backend import app_settings
from backend.lib.sql import models

URL_FRIENDLY_BASE64: LiteralString = ascii_letters + digits + "-_"

SQLAlchemyModel: TypeAlias = Union[DeclarativeBase, DeclarativeAttributeIntercept]


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


def check_if_record_exists_by_id(db: Session, model: SQLAlchemyModel, id: str) -> bool:
    if hasattr(model, "id"):
        exists_criteria: models.Exists = exists().where(model.id == id)
        return db.execute(statement=select(exists_criteria)).scalar() is True
    else:
        raise ValueError(f"Model {model} does not have an 'id' attribute")


def user_id_condition(user_id: str) -> ColumnElement[bool]:
    return or_(
        models.ActiveQuestionnaire.student.has(id=user_id),
        models.ActiveQuestionnaire.teacher.has(id=user_id),
    )


def student_name_condition(student_name: str) -> ColumnElement[bool]:
    return or_(
        models.ActiveQuestionnaire.student.has(
            models.User.user_name.like(other=f"%{student_name}%")
        ),
        models.ActiveQuestionnaire.student.has(
            models.User.full_name.like(other=f"%{student_name}%")
        ),
    )


def teacher_name_condition(teacher_name: str) -> ColumnElement[bool]:
    return or_(
        models.ActiveQuestionnaire.teacher.has(
            models.User.user_name.like(other=f"%{teacher_name}%")
        ),
        models.ActiveQuestionnaire.teacher.has(
            models.User.full_name.like(other=f"%{teacher_name}%")
        ),
    )
