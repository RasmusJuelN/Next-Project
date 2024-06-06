"""
This module contains the authentication logic for the FastAPI application.
"""

from datetime import timedelta, datetime, UTC
from fastapi import Depends, HTTPException, status, Request, APIRouter
from fastapi.security import OAuth2PasswordRequestForm, OAuth2PasswordBearer
from ldap3 import Server, Connection, SASL, DIGEST_MD5, ALL  # type: ignore
from ldap3.core.exceptions import LDAPInvalidCredentialsResult  # type: ignore
from jose import jwt  # type: ignore
from typing import Optional, Any, Union
from logging import Logger, DEBUG, INFO
from pydantic import BaseModel

from lib._logger import LogHelper
from lib._utils import get_full_name_from_ldap

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


oauth2_scheme = OAuth2PasswordBearer(tokenUrl="auth")


router = APIRouter()


@router.post(path="/auth", response_model=dict[str, str])
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
    encoded_jwt: str = await create_access_token(
        data={
            "sub": form_data.username,
            "full_name": full_name,
        },
        expires_delta=access_token_expires,
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
    encoded_jwt: str = jwt.encode(claims=to_encode, key=SECRET_KEY, algorithm=ALGORITHM)
    return encoded_jwt


async def dependency_validate_token(
    token: str = Depends(dependency=oauth2_scheme),
) -> TokenData:
    payload: dict[str, Any] = jwt.decode(
        token=token, key=SECRET_KEY, algorithms=[ALGORITHM]
    )
    username: Union[str, None] = payload.get("sub")

    if username is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid authentication credentials",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return TokenData(username=username)
