from sqlalchemy.ext.asyncio import create_async_engine, AsyncSession, async_sessionmaker
from sqlalchemy.ext.asyncio.engine import AsyncEngine
from typing import Dict, Any

from backend.lib.sql.utils import build_db_connection_args

db_connection_args: Dict[str, Any] = build_db_connection_args()

async_engine: AsyncEngine = create_async_engine(**db_connection_args)

AsyncSessionLocal = async_sessionmaker(bind=async_engine, class_=AsyncSession)
