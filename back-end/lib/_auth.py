"""
This module contains the authentication logic for the FastAPI application.
"""

from datetime import timedelta, datetime, UTC
from fastapi import Depends, HTTPException, status, Request, APIRouter
from fastapi.security import OAuth2PasswordRequestForm, OAuth2PasswordBearer
from ldap3 import Server, Connection, SASL, DIGEST_MD5, ALL  # type: ignore
from ldap3.core.exceptions import LDAPInvalidCredentialsResult  # type: ignore
from jose import jwt  # type: ignore
from typing import Optional, Any, Union, overload
from logging import Logger, DEBUG, INFO
from pydantic import BaseModel

from external.log_helper.log_helper import LogHelper
from lib._utils import (
    get_full_name_from_ldap,
    get_member_of_from_ldap,
    get_uuid_from_ldap,
    determine_scope_from_groups,
    SCOPES,
)

SECRET_KEY = "fcd67c9b07b2d022a3cff8570a1f48b0e73d78abefe3156aa6fde53afacf0210"  # TODO: CHANGE BEFORE DEPLOYMENT AND MOVE TO ENVIRONMENT VARIABLES
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30

DOMAIN = "dc.next.dev"
LDAP_SERVER = f"ldap://{DOMAIN}"
LDAP_BASE_DN = "DC=NEXT,DC=dev"

logger: Logger = LogHelper.create_logger(
    logger_name="backend API (auth)",
    log_file="logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)


class TokenData(BaseModel):
    username: Optional[str] = None
    full_name: Optional[str] = None
    scope: Optional[str] = None
    uuid: Optional[str] = None


oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/api/v1/auth")


class RoleChecker:
    def __init__(self, role: str) -> None:
        self.role = role

    async def __call__(self, token: str = Depends(dependency=oauth2_scheme)) -> None:
        payload: dict[str, Any] = await encode_or_decode_token(token=token)
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


is_admin = RoleChecker(SCOPES["admin"])
is_student = RoleChecker(SCOPES["student"])
is_teacher = RoleChecker(SCOPES["teacher"])


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
                "sub": form_data.username,
                "full_name": full_name,
                "scope": await determine_scope_from_groups(
                    groups=await get_member_of_from_ldap(
                        connection=conn, username=form_data.username
                    )
                ),
                "uuid": await get_uuid_from_ldap(
                    connection=conn, username=form_data.username
                ),
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


async def authenticate_user_ldap(username: str, password: str) -> Connection:
    """
    Low-level function to authenticate a user against an LDAP server.

    Low-level function that attempts to authenticate a user against an LDAP server.
    No exceptions are handled, no logging is done, and no checks are performed.
    Caller is expected to handle all of these. Refer to `ldap3`'s documentation, especially on exceptions.

    Args:
        username (str): The username to authenticate.
        password (str): The password to authenticate.

    Returns:
        Connection: An LDAP connection object on successful authentication.

    Raises:
        LDAPException: If the connection fails for any reason. Use specific exceptions for more granular error handling.
    """
    server = Server(host=LDAP_SERVER, get_info=ALL)

    conn = Connection(
        server=server,
        auto_bind=True,
        version=3,
        authentication=SASL,
        sasl_mechanism=DIGEST_MD5,
        sasl_credentials=(None, username, password, None, "sign"),
        raise_exceptions=True,
    )
    return conn


async def create_access_token(
    data: dict, expires_delta: Optional[timedelta] = None
) -> str:
    to_encode = data.copy()
    if expires_delta:
        expire: datetime = datetime.now(tz=UTC) + expires_delta
    else:
        expire = datetime.now(tz=UTC) + timedelta(minutes=15)
    to_encode.update({"exp": expire})
    encoded_jwt: str = await encode_token(data=to_encode)
    return encoded_jwt


async def dependency_validate_token(
    token: str = Depends(dependency=oauth2_scheme),
) -> TokenData:
    payload: dict[str, Any] = await decode_token(token=token)
    username: Union[str, None] = payload.get("sub")

    if username is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return TokenData(username=username)


async def decode_token(token: str) -> dict[str, Any]:
    return jwt.decode(token=token, key=SECRET_KEY, algorithms=[ALGORITHM])


async def encode_token(data: dict) -> str:
    return jwt.encode(claims=data, key=SECRET_KEY, algorithm=ALGORITHM)


@overload
async def encode_or_decode_token(*, token: str) -> dict[str, Any]: ...  # noqa: E704


@overload
async def encode_or_decode_token(*, data: dict) -> str: ...  # noqa: E704


async def encode_or_decode_token(
    *, token: Optional[str] = None, data: Optional[dict] = None
) -> Union[str, dict[str, Any]]:
    if token is not None and data is None:
        return await decode_token(token=token)
    elif token is None and data is not None:
        return await encode_token(data=data)
    else:
        raise ValueError("Either `token` or `data` must be provided, but not both.")


async def get_full_name_from_token(
    token: str = Depends(dependency=oauth2_scheme),
) -> str:
    payload: dict[str, Any] = await encode_or_decode_token(token=token)
    full_name: Union[str, None] = payload.get("full_name")
    if full_name is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return full_name


async def get_uuid_from_token(
    token: str = Depends(dependency=oauth2_scheme),
) -> str:
    payload: dict[str, Any] = await encode_or_decode_token(token=token)
    uuid: Union[str, None] = payload.get("uuid")
    if uuid is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return uuid


async def get_scope_from_token(
    token: str = Depends(dependency=oauth2_scheme),
) -> str:
    payload: dict[str, Any] = await encode_or_decode_token(token=token)
    scope: Union[str, None] = payload.get("scope")
    if scope is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return scope


async def get_username_from_token(
    token: str = Depends(dependency=oauth2_scheme),
) -> str:
    payload: dict[str, Any] = await encode_or_decode_token(token=token)
    username: Union[str, None] = payload.get("sub")
    if username is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return username


async def get_token_data(
    token: str = Depends(dependency=oauth2_scheme),
) -> TokenData:
    payload: dict[str, Any] = await encode_or_decode_token(token=token)
    return TokenData(
        username=payload.get("sub"),
        full_name=payload.get("full_name"),
        scope=payload.get("scope"),
        uuid=payload.get("uuid"),
    )
