from sqlalchemy.ext.asyncio import create_async_engine, AsyncSession, async_sessionmaker
from sqlalchemy.ext.asyncio.engine import AsyncEngine
from sqlalchemy.orm import Session, sessionmaker
from sqlalchemy.engine import Engine, create_engine
from typing import Dict, Any

from backend.lib.sql.utils import build_db_connection_args

async_db_connection_args: Dict[str, Any] = build_db_connection_args(async_driver=True)

async_engine: AsyncEngine = create_async_engine(**async_db_connection_args)

AsyncSessionLocal = async_sessionmaker(
    bind=async_engine, class_=AsyncSession, autoflush=False, autocommit=False
)

# We need non-async engine and session for certain operations, like creating tables
db_connection_args: Dict[str, Any] = build_db_connection_args(async_driver=False)

engine: Engine = create_engine(**db_connection_args)

sessionLocal = sessionmaker(
    bind=engine, class_=Session, autoflush=False, autocommit=False
)
