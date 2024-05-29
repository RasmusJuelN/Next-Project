from logging import DEBUG, INFO, Logger
from fastapi import FastAPI, APIRouter, HTTPException, Depends, status, Request
from fastapi.responses import JSONResponse
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm
from jose import JWTError, jwt, ExpiredSignatureError  # type: ignore
from ldap3 import Server, Connection, ALL, NTLM  # type: ignore
from ldap3.core.exceptions import (  # type: ignore
    LDAPException,
    LDAPInvalidCredentialsResult,
    LDAPSocketOpenError,
)
from typing import Literal, Optional, Any, Union
from pydantic import BaseModel
from datetime import datetime, timedelta, UTC

from lib._logger import LogHelper

SECRET_KEY = Literal[
    "fcd67c9b07b2d022a3cff8570a1f48b0e73d78abefe3156aa6fde53afacf0210"
]  # TODO: CHANGE BEFORE DEPLOYMENT
ALGORITHM = Literal["HS256"]
ACCESS_TOKEN_EXPIRE_MINUTES = 30

LDAP_SERVER = "ldap://placeholder"
LDAP_BASE_DN = "DC=placeholder,DC=placeholder"

logger: Logger = LogHelper.create_logger(
    logger_name="backend API",
    log_file="logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)


class TokenData(BaseModel):
    username: Optional[str] = None


oauth2_scheme = OAuth2PasswordBearer(tokenUrl="auth")


app = FastAPI()


@app.exception_handler(exc_class_or_status_code=HTTPException)
async def http_exception_handler(request: Request, exc: HTTPException) -> JSONResponse:
    return JSONResponse(
        status_code=exc.status_code,
        content={"detail": exc.detail},
    )


@app.exception_handler(exc_class_or_status_code=LDAPException)
async def ldap_exception_handler(request: Request, exc: LDAPException) -> JSONResponse:
    if isinstance(exc, LDAPInvalidCredentialsResult):
        logger.info(msg=f"Invalid credentials for user {exc.connection.user}")
        return JSONResponse(
            status_code=status.HTTP_401_UNAUTHORIZED,
            content={"detail": "Incorrect username or password"},
        )
    elif isinstance(exc, LDAPSocketOpenError):
        logger.error(
            msg="Timed out trying to connect to server.",
            exc_info=True,
        )
        return JSONResponse(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            content={"detail": "Timed out trying to connect to server"},
        )
    else:
        logger.error(msg=f"Unexpected error occurred: {exc}")
        return JSONResponse(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            content={
                "detail": "An unexpected error occurred while trying to authenticate. Please try again later."
            },
        )


@app.exception_handler(exc_class_or_status_code=JWTError)
async def jwt_exception_handler(request: Request, exc: JWTError) -> JSONResponse:
    if isinstance(exc, ExpiredSignatureError):
        return JSONResponse(
            status_code=status.HTTP_401_UNAUTHORIZED,
            content={"detail": "Token has expired"},
        )
    else:
        return JSONResponse(
            status_code=status.HTTP_401_UNAUTHORIZED,
            content={"detail": "Invalid token"},
        )


router = APIRouter()


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


@router.post(path="/auth", response_model=dict)
async def authenticate_user(
    form_data: OAuth2PasswordRequestForm = Depends(),
) -> dict[str, str]:
    user: Connection = await authenticate_user_ldap(
        username=form_data.username, password=form_data.password
    )
    if not user:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Incorrect username or password",
            headers={"WWW-Authenticate": "Bearer"},
        )
    access_token_expires = timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    encoded_jwt: str = await create_access_token(
        data={"sub": form_data.username}, expires_delta=access_token_expires
    )
    return {"access_token": encoded_jwt, "token_type": "bearer"}


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
    user_dn: str = f"CN={username},{LDAP_BASE_DN}"

    conn = Connection(
        server=server,
        user=user_dn,
        password=password,
        authentication=NTLM,
        auto_bind=True,
        raise_exceptions=True,
    )
    return conn


@app.get(path="/", response_model=dict)
async def read_root() -> dict[str, str]:
    return {"message": "Hello World"}


@app.get(path="/protected", response_model=dict)
async def read_protected(
    token: str = Depends(dependency=oauth2_scheme),
    username: TokenData = Depends(dependency=dependency_validate_token),
) -> dict[str, str]:
    return {"message": "You are authenticated"}


app.include_router(router=router, prefix="/api/v1")
