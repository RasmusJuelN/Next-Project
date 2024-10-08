"""
This module contains the authentication logic for the FastAPI application.
"""

from datetime import timedelta
from typing import Sequence, Annotated
from fastapi import Depends, HTTPException, status, Request, APIRouter
from fastapi.security import OAuth2PasswordRequestForm
from ldap3.core.exceptions import LDAPInvalidCredentialsResult
from ldap3 import Connection

from backend.lib.api.auth import logger
from backend.lib.api.auth.utility import (
    get_full_name_from_ldap,
    get_member_of_from_ldap,
    get_uuid_from_ldap,
    determine_scope_from_groups,
    create_access_token,
    query_for_users_ldap,
    hash_string,
)
from backend.lib.api.auth.dependencies import authenticate_with_sa_service_account
from backend.lib.api.auth.models import User, UserSearchResponse, UserSearchRequest
from backend.lib.api.auth.exceptions import NoLDAPResultsError
from backend.lib.api.auth.classes import LDAPConnection
from backend import app_settings


router = APIRouter()


@router.post(path="/auth", tags=["auth"])
def authenticate_user(
    request: Request,
    form_data: Annotated[OAuth2PasswordRequestForm, Depends()],
) -> dict[str, str]:
    try:
        with LDAPConnection(
            username=form_data.username,
            password=form_data.password,
        ) as connection:

            access_token_expires = timedelta(
                minutes=app_settings.settings.auth.access_token_expire_minutes
            )
            full_name: str = get_full_name_from_ldap(
                connection=connection, username=form_data.username
            )
            encoded_jwt: str = create_access_token(
                data={
                    "sub": hash_string(
                        string=get_uuid_from_ldap(
                            connection=connection, username=form_data.username
                        )
                    ),
                    "full_name": full_name,
                    "scope": determine_scope_from_groups(
                        groups=get_member_of_from_ldap(
                            connection=connection, username=form_data.username
                        )
                    ),
                    "username": form_data.username,
                },
                expires_delta=access_token_expires,
            )

    except ValueError as e:
        logger.exception(msg=str(object=e))
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="User does not have the required permissions to access this resource.",
            headers={"WWW-Authenticate": "Bearer"},
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

    return {"access_token": encoded_jwt, "token_type": "bearer"}


@router.get(path="/users/search", tags=["auth"], response_model=UserSearchResponse)
def query_for_users(
    request: Request,
    connection: Annotated[
        Connection, Depends(dependency=authenticate_with_sa_service_account)
    ],
    query_params: Annotated[UserSearchRequest, Depends()],
) -> UserSearchResponse:
    try:
        users: Sequence[User] = query_for_users_ldap(
            connection=connection,
            role=query_params.role,
            search_query=query_params.search_query,
            page=query_params.page,
            limit=query_params.limit,
        )
    except NoLDAPResultsError:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="No users were found matching the search criteria.",
        )

    return UserSearchResponse(users=users)
