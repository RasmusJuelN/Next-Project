from typing import Any, Union, overload, Optional
from fastapi import HTTPException, status, Depends
from jose import jwt
from ldap3 import Server, Connection, SASL, DIGEST_MD5, ALL
from datetime import datetime, timedelta, UTC

from .constants import ALGORITHM, SECRET_KEY, LDAP_SERVER
from .dependencies import oauth2_scheme


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
    encoded_jwt: str = await encode_or_decode_token(data=to_encode)
    return encoded_jwt
