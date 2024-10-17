from sqlalchemy.orm import Session, sessionmaker
from sqlalchemy.engine import Engine, create_engine
from typing import Dict, Any

from backend.lib.sql.utils import build_db_connection_args

db_connection_args: Dict[str, Any] = build_db_connection_args()

engine: Engine = create_engine(**db_connection_args)

sessionLocal = sessionmaker(
    bind=engine, class_=Session, autoflush=True, autocommit=False
)
