from typing import Any, Generator
from sqlalchemy.orm.session import Session

from backend.lib.sql.database import sessionLocal


def get_db() -> Generator[Session, Any, None]:
    """
    Dependency that provides a SQLAlchemy database session.

    Yields:
        Generator[Session, Any, None]: A SQLAlchemy Session object.

    This function is used as a dependency in FastAPI routes to provide a database session.
    It ensures that the session is properly closed after the request is processed.
    """
    db: Session = sessionLocal()
    try:
        yield db
    finally:
        db.close()
