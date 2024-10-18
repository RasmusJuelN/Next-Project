from urllib import parse
from typing import Dict, Any, Literal, LiteralString, TypeAlias, Union
from string import ascii_letters, digits
from sqlalchemy.orm import Session, DeclarativeBase
from sqlalchemy.orm.decl_api import DeclarativeAttributeIntercept
from sqlalchemy.sql import exists, select, or_, ColumnElement

from backend import app_settings
from backend.lib.sql import models, schemas

URL_FRIENDLY_BASE64: LiteralString = ascii_letters + digits + "-_"

SQLAlchemyModel: TypeAlias = Union[DeclarativeBase, DeclarativeAttributeIntercept]
OptionSchemaType: TypeAlias = Union[
    schemas.CreateOptionModel, schemas.UpdateOptionModel
]
QuestionSchemaType: TypeAlias = Union[
    schemas.CreateQuestionModel, schemas.UpdateQuestionModel
]


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
    """
    Check if a record exists in the database by its ID.

    Args:
        db (Session): The SQLAlchemy session to use for the database query.
        model (SQLAlchemyModel): The SQLAlchemy model class to query.
        id (str): The ID of the record to check for existence.

    Returns:
        bool: True if the record exists, False otherwise.

    Raises:
        ValueError: If the provided model does not have an 'id' attribute.
    """
    if hasattr(model, "id"):
        exists_criteria: models.Exists = exists().where(model.id == id)
        return db.execute(statement=select(exists_criteria)).scalar() is True
    else:
        raise ValueError(f"Model {model} does not have an 'id' attribute")


def user_id_condition(user_id: str) -> ColumnElement[bool]:
    """
    Generates a SQLAlchemy condition to check if a given user ID matches either a student or teacher
    in the ActiveQuestionnaire model.

    Args:
        user_id (str): The user ID to check against the ActiveQuestionnaire model.

    Returns:
        ColumnElement[bool]: A SQLAlchemy condition that evaluates to True if the user ID matches
                             either a student or teacher in the ActiveQuestionnaire model.
    """
    return or_(
        models.ActiveQuestionnaire.student.has(id=user_id),
        models.ActiveQuestionnaire.teacher.has(id=user_id),
    )


def student_name_condition(student_name: str) -> ColumnElement[bool]:
    """
    Generates a SQLAlchemy condition to filter ActiveQuestionnaire entries by student name.

    This function creates a condition that checks if the student's username or full name
    contains the specified `student_name` substring. It uses the SQLAlchemy `or_` function
    to combine the conditions.

    Args:
        student_name (str): The substring to search for in the student's username or full name.

    Returns:
        ColumnElement[bool]: A SQLAlchemy condition that can be used in a query to filter
                             ActiveQuestionnaire entries by student name.
    """
    return or_(
        models.ActiveQuestionnaire.student.has(
            models.User.user_name.like(other=f"%{student_name}%")
        ),
        models.ActiveQuestionnaire.student.has(
            models.User.full_name.like(other=f"%{student_name}%")
        ),
    )


def teacher_name_condition(teacher_name: str) -> ColumnElement[bool]:
    """
    Generates a SQLAlchemy condition to filter ActiveQuestionnaire entries by teacher name.

    This function creates a condition that checks if the teacher's username or full name
    contains the specified `teacher_name` substring. It uses the SQLAlchemy `or_` function
    to combine the conditions.

    Args:
        teacher_name (str): The substring to search for in the teacher's username or full name.

    Returns:
        ColumnElement[bool]: A SQLAlchemy condition that can be used in a query to filter
                             ActiveQuestionnaire entries by teacher name.
    """
    return or_(
        models.ActiveQuestionnaire.teacher.has(
            models.User.user_name.like(other=f"%{teacher_name}%")
        ),
        models.ActiveQuestionnaire.teacher.has(
            models.User.full_name.like(other=f"%{teacher_name}%")
        ),
    )


def create_option_model(schema: OptionSchemaType, question_id: int) -> models.Option:
    """
    Creates an Option model instance from the provided schema and question ID.

    Args:
        schema (OptionSchemaType): The schema containing the option details.
        question_id (int): The ID of the question to which the option belongs.

    Returns:
        models.Option: The created Option model instance.
    """
    return models.Option(
        question_id=question_id,
        value=schema.value,
        label=schema.label,
        is_custom=schema.is_custom,
    )


def create_question_model(
    schema: QuestionSchemaType, template_id: str
) -> models.Question:
    """
    Create a Question model instance from the given schema and template ID.

    Args:
        schema (QuestionSchemaType): The schema containing the question details.
        template_id (str): The ID of the template to associate with the question.

    Returns:
        models.Question: The created Question model instance.
    """
    return models.Question(
        template_id=template_id,
        title=schema.title,
    )


def create_user_schema(user: models.User) -> schemas.User:
    """
    Converts a User model instance to a User schema instance.

    Args:
        user (models.User): The User model instance to be converted.

    Returns:
        schemas.User: The corresponding User schema instance.
    """
    return schemas.User(
        id=user.id,
        user_name=user.user_name,
        full_name=user.full_name,
        role=user.role,
    )
