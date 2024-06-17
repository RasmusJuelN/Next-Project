from typing import Any, Union
from fastapi import Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer
from jose import jwt

from backend.lib._utils import SCOPES
from .models import TokenData
from .constants import ALGORITHM, SECRET_KEY

oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/api/v1/auth")


class RoleChecker:
    """
    RoleChecker class is responsible for checking if a user has the required role to access a resource.

    Refer to the `SCOPES` constant in lib._utils for the available roles. No value is returned.

    Raises:
        HTTPException (status_code=401): If the token is invalid.
        HTTPException (status_code=403): If the user does not have the required role.

    Example:
        ```python
        is_admin = RoleChecker(SCOPES["admin"])

        @router.get("/admin", dependencies=[Depends(is_admin)])
        async def admin_only_route():
            return {"message": "You are an admin."}
        ```

    Alternatively:
        ```python
        is_admin = RoleChecker(SCOPES["admin"])

        @router.get("/admin")
        async def admin_only_route(_ = Depends(is_admin)):
            return {"message": "You are an admin."}
        ```
    """

    def __init__(self, role: str) -> None:
        self.role: str = role

    async def __call__(self, token: str = Depends(dependency=oauth2_scheme)) -> None:
        payload: dict[str, Any] = jwt.decode(
            token=token, key=SECRET_KEY, algorithms=[ALGORITHM]
        )
        scope: Union[str, None] = payload.get("scope")
        if scope is None:
            raise HTTPException(
                status_code=status.HTTP_401_UNAUTHORIZED,
                detail="Invalid token",
                headers={"WWW-Authenticate": "Bearer"},
            )
        if not scope == self.role:
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail="You do not have the required permissions to access this resource.",
            )


is_admin = RoleChecker(role=SCOPES["admin"])
is_student = RoleChecker(role=SCOPES["student"])
is_teacher = RoleChecker(role=SCOPES["teacher"])


async def get_token_data(
    token: str = Depends(dependency=oauth2_scheme),
) -> TokenData:
    payload: dict[str, Any] = jwt.decode(
        token=token, key=SECRET_KEY, algorithms=[ALGORITHM]
    )
    return TokenData(
        username=payload.get("sub"),
        full_name=payload.get("full_name"),
        scope=payload.get("scope"),
        uuid=payload.get("uuid"),
    )


async def validate_token(
    token: str = Depends(dependency=oauth2_scheme),
) -> TokenData:
    payload: dict[str, Any] = jwt.decode(
        token=token, key=SECRET_KEY, algorithms=[ALGORITHM]
    )
    username: Union[str, None] = payload.get("sub")

    if username is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return TokenData(username=username)
