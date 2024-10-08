from typing import Any, Union, Optional, Annotated
from fastapi import Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer
from jose import jwt
from ldap3 import Connection

from backend.lib.api.auth.models import TokenData
from backend.lib.api.auth.exceptions import (
    MissingADServiceAccountError,
    MissingSecretKeyError,
)
from backend.lib.api.auth.classes import LDAPConnection
from backend import app_settings

oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/api/v1/auth")


def get_token_data(
    token: Annotated[str, Depends(dependency=oauth2_scheme)],
) -> TokenData:
    """
    Retrieves the token data from the provided token.

    Args:
        token (str): The token to decode and retrieve data from.

    Returns:
        TokenData: An instance of the TokenData class containing the decoded token data.
    """
    if app_settings.settings.auth.secret_key is None:
        raise MissingSecretKeyError()

    payload: dict[str, Any] = jwt.decode(
        token=token,
        key=app_settings.settings.auth.secret_key,
        algorithms=[app_settings.settings.auth.algorithm],
    )
    return TokenData(
        username=payload.get("username", "N/A"),
        full_name=payload.get("full_name", "N/A"),
        scope=payload.get("scope", "N/A"),
        uuid=payload.get("sub", "N/A"),
    )


def validate_token(
    token: Annotated[str, Depends(dependency=oauth2_scheme)],
) -> TokenData:
    """
    Validates the provided token and returns the decoded token data.

    Args:
        token (str): The token to be validated.

    Returns:
        TokenData: The decoded token data.

    Raises:
        HTTPException: If the token is invalid or missing a username.
    """
    if app_settings.settings.auth.secret_key is None:
        raise MissingSecretKeyError()

    payload: dict[str, Any] = jwt.decode(
        token=token,
        key=app_settings.settings.auth.secret_key,
        algorithms=[app_settings.settings.auth.algorithm],
    )
    uuid: Union[str, None] = payload.get("sub")
    full_name: Union[str, None] = payload.get("full_name")
    scope: Union[str, None] = payload.get("scope")
    username: Union[str, None] = payload.get("username")

    if username is None or uuid is None or full_name is None or scope is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return TokenData(
        username=username,
        full_name=full_name,
        scope=scope,
        uuid=uuid,
    )


def is_student(
    token_data: Annotated[TokenData, Depends(dependency=validate_token)]
) -> bool:
    """
    Checks if the user is a student.

    Args:
        token_data (TokenData): The decoded token data.

    Returns:
        bool: True if the user is a student, False otherwise.
    """
    student: str = app_settings.settings.auth.scopes["student"]
    if student not in token_data.scope:
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="User does not have the required permissions to access this resource.",
        )
    return True


def is_teacher(
    token_data: Annotated[TokenData, Depends(dependency=validate_token)]
) -> bool:
    """
    Checks if the user is a teacher.

    Args:
        token_data (TokenData): The decoded token data.

    Returns:
        bool: True if the user is a teacher, False otherwise.
    """
    teacher: str = app_settings.settings.auth.scopes["teacher"]
    if teacher not in token_data.scope:
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="User does not have the required permissions to access this resource.",
        )
    return True


def is_admin(
    token_data: Annotated[TokenData, Depends(dependency=validate_token)]
) -> bool:
    """
    Checks if the user is an admin.

    Args:
        token_data (TokenData): The decoded token data.

    Returns:
        bool: True if the user is an admin, False otherwise.
    """
    admin: str = app_settings.settings.auth.scopes["admin"]
    if admin not in token_data.scope:
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="User does not have the required permissions to access this resource.",
        )
    return True


def authenticate_with_sa_service_account() -> Connection:
    """
    Authenticates with the Active Directory (AD) using a service account.

    This function establishes a connection to the LDAP server using the
    service account credentials specified in the application settings.
    It uses SASL authentication with the DIGEST-MD5 mechanism.

    Returns:
        Connection: An LDAP connection object.

    Raises:
        MissingADServiceAccountError: If the AD service account or password is not provided in the application settings.

    Note:
        Auto binds the connection to the server immediately after creation.
    """
    ad_service_account: Optional[str] = app_settings.settings.auth.ad_service_account
    ad_service_password: Optional[str] = app_settings.settings.auth.ad_service_password

    if ad_service_account is None or ad_service_password is None:
        raise MissingADServiceAccountError()

    conn: Connection = LDAPConnection(
        username=ad_service_account,
        password=ad_service_password,
    ).authenticate()

    return conn
