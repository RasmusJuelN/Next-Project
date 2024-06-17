"""
This module contains the authentication logic for the FastAPI application.
"""

from datetime import timedelta
from fastapi import Depends, HTTPException, status, Request, APIRouter
from fastapi.security import OAuth2PasswordRequestForm
from ldap3.core.exceptions import LDAPInvalidCredentialsResult  # type: ignore
from logging import Logger, DEBUG, INFO

from backend.lib._logger import LogHelper
from .utility import (
    get_full_name_from_ldap,
    get_member_of_from_ldap,
    get_uuid_from_ldap,
    determine_scope_from_groups,
)
from .constants import ACCESS_TOKEN_EXPIRE_MINUTES
from .utility import authenticate_user_ldap, create_access_token


logger: Logger = LogHelper.create_logger(
    logger_name="backend API (auth)",
    log_file="backend/logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)


router = APIRouter()


@router.post(path="/auth", tags=["auth"])
async def authenticate_user(
    request: Request,
    form_data: OAuth2PasswordRequestForm = Depends(),
) -> dict[str, str]:
    try:
        conn = await authenticate_user_ldap(
            username=form_data.username, password=form_data.password
        )
    except LDAPInvalidCredentialsResult:
        ip_address: str = (
            request.client.host if request.client else "Unknown IP Address"
        )
        logger.info(
            msg=f"{ip_address} failed to authenticate with username {form_data.username}"
        )
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Incorrect username or password",
            headers={"WWW-Authenticate": "Bearer"},
        )
    access_token_expires = timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    full_name: str = await get_full_name_from_ldap(
        connection=conn, username=form_data.username
    )
    try:
        encoded_jwt: str = await create_access_token(
            data={
                "sub": await get_uuid_from_ldap(
                    connection=conn, username=form_data.username
                ),
                "full_name": full_name,
                "scope": await determine_scope_from_groups(
                    groups=await get_member_of_from_ldap(
                        connection=conn, username=form_data.username
                    )
                ),
                "username": form_data.username,
            },
            expires_delta=access_token_expires,
        )
    except ValueError as e:
        conn.unbind()
        logger.exception(msg=str(object=e))
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="User does not have the required permissions to access this resource.",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return {"access_token": encoded_jwt, "token_type": "bearer"}
