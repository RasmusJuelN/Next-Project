from typing import AsyncGenerator
from sqlalchemy.ext.asyncio import AsyncSession

from backend.lib.sql.database import AsyncSessionLocal


async def get_db() -> AsyncGenerator[AsyncSession, None]:
    """
    Get the database session.

    Returns:
        AsyncSession: The database session.
    """
    db: AsyncSession = AsyncSessionLocal()
    try:
        yield db
    finally:
        await db.close()
