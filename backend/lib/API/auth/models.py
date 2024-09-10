from pydantic import BaseModel
from typing import Optional


class TokenData(BaseModel):
    username: Optional[str] = None
    full_name: Optional[str] = None
    scope: Optional[str] = None
    uuid: Optional[str] = None
