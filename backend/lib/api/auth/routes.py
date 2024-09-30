"""
This module contains the authentication logic for the FastAPI application.
"""

from datetime import timedelta
from fastapi import Depends, HTTPException, status, Request, APIRouter, Query
from fastapi.security import OAuth2PasswordRequestForm
from ldap3.core.exceptions import LDAPInvalidCredentialsResult
from ldap3 import Connection
from logging import Logger, DEBUG, INFO
from typing import Sequence

from backend.lib._logger import LogHelper
from backend.lib.api.auth.utility import (
    get_full_name_from_ldap,
    get_member_of_from_ldap,
    get_uuid_from_ldap,
    determine_scope_from_groups,
    authenticate_user_ldap,
    create_access_token,
)
from backend.lib.api.auth.models import User
from backend import app_settings


logger: Logger = LogHelper.create_logger(
    logger_name="backend API (auth)",
    log_file="backend/logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)

router = APIRouter()


@router.post(path="/auth", tags=["auth"])
def authenticate_user(
    request: Request,
    form_data: OAuth2PasswordRequestForm = Depends(),
) -> dict[str, str]:
    try:
        conn: Connection = authenticate_user_ldap(
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
    access_token_expires = timedelta(
        minutes=app_settings.settings.auth.access_token_expire_minutes
    )
    full_name: str = get_full_name_from_ldap(
        connection=conn, username=form_data.username
    )
    try:
        encoded_jwt: str = create_access_token(
            data={
                "sub": get_uuid_from_ldap(connection=conn, username=form_data.username),
                "full_name": full_name,
                "scope": determine_scope_from_groups(
                    groups=get_member_of_from_ldap(
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


@router.get(path="/users", tags=["auth"], response_model=Sequence[User])
def query_for_users(
    request: Request,
    role: str,
    search_query: str,
    page: int,
    limit: int = Query(default=..., le=10),
) -> Sequence[User]:
    raise NotImplementedError
