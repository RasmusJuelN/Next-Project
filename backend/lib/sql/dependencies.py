from typing import Any, Generator
from sqlalchemy.orm.session import Session

from backend.lib.sql.database import sessionLocal


def get_db() -> Generator[Session, Any, None]:
    """
    Dependency that provides a SQLAlchemy database session.

    Yields:
        Generator[Session, Any, None]: A SQLAlchemy Session object.

    This function is used as a dependency in FastAPI routes to provide
    a database session. It ensures that the session is properly closed
    after the request is processed.

    Note:
        The session is yielded as a context manager, so it is automatically
        closed after the request is processed. It does NOT, however,
        automatically commit or rollback the transaction. This must be done
        explicitly with `session.commit()`|`session.rollback()`, or by using
        the yielded session as a context manager with `with session.begin():`.

    Example:
        ```
        from backend.lib.sql.dependencies import get_db

        session = get_db()

        with session.begin():
            session.add(some_object)
        ```
    """
    with sessionLocal() as session:
        yield session
