from typing import Any, Union, overload, Optional, List, Sequence, cast, Annotated
from fastapi import HTTPException, status, Depends
from jose import jwt
from ldap3 import Connection, Entry, SUBTREE
from datetime import datetime, timedelta, UTC
from hashlib import sha512

from backend import app_settings
from backend.lib.api.auth.dependencies import oauth2_scheme
from backend.lib.api.auth.exceptions import (
    MissingSecretKeyError,
    NoLDAPResultsError,
    MissingSaltHashError,
)
from backend.lib.api.auth.models import User
from backend.lib.api.auth.typing import UserSearchEntry
from backend.lib import cache


def hash_string(string: str) -> str:
    """
    Hashes a string using SHA-512 with a salt.

    Args:
        string (str): The string to hash.

    Raises:
        MissingSaltHashError: If the salt hash is missing in the settings file.

    Returns:
        str: The hashed string.
    """
    if app_settings.settings.auth.salt_hash is None:
        raise MissingSaltHashError

    return sha512(
        string=f"{string}{app_settings.settings.auth.salt_hash}".encode()
    ).hexdigest()


def decode_token(token: str) -> dict[str, Any]:
    """
    Convenience function to decode a token. Wraps around `jwt.decode`.

    Args:
        token (str): The token to decode.

    Returns:
        dict[str, Any]: The payload extracted from the token.

    Raises:
        Refer to `jwt.decode` for possible exceptions.
    """
    if app_settings.settings.auth.secret_key is None:
        raise MissingSecretKeyError()
    return jwt.decode(
        token=token,
        key=app_settings.settings.auth.secret_key,
        algorithms=[app_settings.settings.auth.algorithm],
    )


def encode_token(data: dict) -> str:
    """
    Convenience function to encode a token. Wraps around `jwt.encode`.

    Args:
        data (dict): The data to encode.

    Returns:
        str: The encoded token.

    Raises:
        Refer to `jwt.encode` for possible exceptions.
    """
    if app_settings.settings.auth.secret_key is None:
        raise MissingSecretKeyError()
    return jwt.encode(
        claims=data,
        key=app_settings.settings.auth.secret_key,
        algorithm=app_settings.settings.auth.algorithm,
    )


@overload
def encode_or_decode_token(*, token: str) -> dict[str, Any]:
    """
    Decode the provided token and return the payload.

    Args:
        token (str): The token to decode.

    Returns:
        dict[str, Any]: The payload extracted from the token.

    Raises:
        Refer to `jwt.decode` for possible exceptions.
    """
    ...


@overload
def encode_or_decode_token(*, data: dict) -> str:
    """
    Encode the provided data and return the token.

    Args:
        data (dict): The data to encode.

    Returns:
        str: The encoded token.

    Raises:
        Refer to `jwt.encode` for possible exceptions.
    """
    ...


def encode_or_decode_token(
    *, token: Optional[str] = None, data: Optional[dict] = None
) -> Union[str, dict[str, Any]]:
    """
    Encode or decode the provided token or data.

    Args:
        token (Optional[str]): The token to decode.
        data (Optional[dict]): The data to encode.

    Returns:
        Union[str, dict[str, Any]]: The decoded payload or the encoded token.

    Raises:
        ValueError: If both `token` and `data` are provided.
        Refer to `jwt.decode` and `jwt.encode` for possible exceptions.
    """
    if token is not None and data is None:
        return decode_token(token=token)
    elif token is None and data is not None:
        return encode_token(data=data)
    else:
        raise ValueError("Either `token` or `data` must be provided, but not both.")


def get_full_name_from_token(
    token: Annotated[str, Depends(dependency=oauth2_scheme)],
) -> str:
    """
    Retrieves the full name from the provided token.

    Args:
        token (str): The token to decode and extract the full name from.

    Returns:
        str: The full name extracted from the token.

    Raises:
        HTTPException: If the token is invalid or does not contain a full name.
    """
    payload: dict[str, Any] = encode_or_decode_token(token=token)
    full_name: Union[str, None] = payload.get("full_name")
    if full_name is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return full_name


def get_uuid_from_token(
    token: Annotated[str, Depends(dependency=oauth2_scheme)],
) -> str:
    """
    Retrieves the UUID from the provided token.

    Args:
        token (str): The token to decode and extract the UUID from.

    Returns:
        str: The UUID extracted from the token.

    Raises:
        HTTPException: If the token is invalid or does not contain a UUID.
    """
    payload: dict[str, Any] = encode_or_decode_token(token=token)
    uuid: Union[str, None] = payload.get("uuid")
    if uuid is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return uuid


def get_scope_from_token(
    token: Annotated[str, Depends(dependency=oauth2_scheme)],
) -> str:
    """
    Retrieves the scope from the provided token.

    Args:
        token (str): The token to extract the scope from.

    Returns:
        str: The scope extracted from the token.

    Raises:
        HTTPException: If the token is invalid or does not contain a scope.
    """
    payload: dict[str, Any] = encode_or_decode_token(token=token)
    scope: Union[str, None] = payload.get("scope")
    if scope is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return scope


def get_username_from_token(
    token: Annotated[str, Depends(dependency=oauth2_scheme)],
) -> str:
    """
    Retrieves the username from the provided token.

    Args:
        token (str): The token to decode and extract the username from.

    Returns:
        str: The username extracted from the token.

    Raises:
        HTTPException: If the token is invalid or does not contain a username.
    """
    payload: dict[str, Any] = encode_or_decode_token(token=token)
    username: Union[str, None] = payload.get("sub")
    if username is None:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid token",
            headers={"WWW-Authenticate": "Bearer"},
        )
    return username


def create_access_token(data: dict, expires_delta: Optional[timedelta] = None) -> str:
    """
    Create an access token with the provided data and expiration delta.

    Args:
        data (dict): The data to be encoded in the access token.
        expires_delta (Optional[timedelta]): The expiration delta for the access token.
            If not provided, a default expiration of 15 minutes will be used.

    Returns:
        str: The encoded access token.
    """
    to_encode = data.copy()
    if expires_delta:
        expire: datetime = datetime.now(tz=UTC) + expires_delta
    else:
        expire = datetime.now(tz=UTC) + timedelta(minutes=15)
    to_encode.update({"exp": expire})
    encoded_jwt: str = encode_or_decode_token(data=to_encode)
    return encoded_jwt


def get_object_by_uuid(
    connection: Connection,
    uuid: str,
) -> str:
    """
    Retrieves the object by UUID from LDAP.

    Args:
        connection (Connection): The LDAP connection object.
        uuid (str): The UUID of the object to retrieve.

    Returns:
        str: The object retrieved by UUID.

    Raises:
        ValueError: If no matching entries are found or if the 'objectGUID' attribute is not found.
    """
    try:
        connection.search(
            search_base=app_settings.settings.auth.ldap_base_dn,
            search_filter=f"(objectGUID={uuid})",
            attributes=["sAMAccountName"],
        )
    except Exception as e:
        raise RuntimeError("LDAP search failed") from e

    if len(connection.entries) == 0:
        raise ValueError("No matching entries found")

    if "sAMAccountName" in connection.entries[0]:
        return connection.entries[0]["sAMAccountName"].value

    raise ValueError("The 'sAMAccountName' attribute was not found")


def determine_scope_from_groups(groups: List[str]) -> str:
    """
    Determines the scope of a user based on the groups they are a member of.

    Args:
        groups (List[str]): The list of groups that the user is a member of.

    Returns:
        str: The scope of the user.

    Raises:
        ValueError: If no matching scopes are found.
    """
    # The group is a Distinguished Name (DN) and we only want the Common Name (CN), i.e. 'CN=elev,OU=groups,OU=next,DC=NEXT,DC=dev' -> 'elev'
    groups = [group.split(",")[0].split("=")[1] for group in groups]

    for group in groups:
        if group in app_settings.settings.auth.scopes:
            return app_settings.settings.auth.scopes[group]

    raise ValueError("No matching scopes found")


def get_uuid_from_ldap(
    connection: Connection,
    username: str,
) -> str:
    """
    Retrieves the UUID of a user from LDAP based on their username.

    Args:
        connection (Connection): The LDAP connection object.
        username (str): The username of the user.

    Returns:
        str: The UUID of the user.

    Raises:
        ValueError: If no matching entries are found or if the 'objectGUID' attribute is not found.
    """
    try:
        connection.search(
            search_base=app_settings.settings.auth.ldap_base_dn,
            search_filter=f"(sAMAccountName={username})",
            attributes=["objectGUID"],
        )
    except Exception as e:
        raise RuntimeError("LDAP search failed") from e

    if len(connection.entries) == 0:
        raise ValueError("No matching entries found")

    try:
        entry: Entry = connection.entries[0]
        id: str = entry.objectGUID.value
        return id
    except AttributeError:
        raise ValueError("The 'objectGUID' attribute was not found")


def get_member_of_from_ldap(
    connection: Connection,
    username: str,
) -> List[str]:
    """
    Retrieves the groups that a user is a member of from LDAP based on their username.

    Args:
        connection (Connection): The LDAP connection object.
        username (str): The username of the user.

    Returns:
        List[str]: The list of groups that the user is a member of.

    Raises:
        ValueError: If no matching entries are found or if the 'memberOf' attribute is not found.
    """
    try:
        connection.search(
            search_base=app_settings.settings.auth.ldap_base_dn,
            search_filter=f"(sAMAccountName={username})",
            attributes=["memberOf"],
        )
    except Exception as e:
        raise RuntimeError("LDAP search failed") from e

    if len(connection.entries) == 0:
        raise ValueError("No matching entries found")

    if "memberOf" in connection.entries[0]:
        return connection.entries[0]["memberOf"].values

    raise ValueError("The 'memberOf' attribute was not found")


def get_full_name_from_ldap(
    connection: Connection,
    username: str,
) -> str:
    """
    Retrieves the full name of a user from LDAP based on their username.

    Args:
        connection (Connection): The LDAP connection object.
        username (str): The username of the user.

    Returns:
        str: The full name of the user.

    Raises:
        ValueError: If no matching entries are found or if none of the expected attributes are found.

    Notes:
        Refer to return_first_non_empty_attribute for more information.
    """
    return return_first_non_empty_attribute(
        connection=connection,
        username=username,
        attributes=[
            "displayName",
            "cn",
            "name",
            "givenName",
        ],
    )


def return_first_non_empty_attribute(
    connection: Connection, username: str, *, attributes: List[str]
) -> str:
    """
    Retrieves the first non-empty attribute of a user from LDAP based on their username.

    Args:
        connection (Connection): The LDAP connection object.
        username (str): The username of the user.
        attributes (List[str]): The list of attributes to search for.

    Returns:
        str: The first non-empty attribute of the user.

    Raises:
        ValueError: If no matching entries are found or if none of the expected attributes are found.
    """
    try:
        connection.search(
            search_base=app_settings.settings.auth.ldap_base_dn,
            search_filter=f"(sAMAccountName={username})",
            attributes=attributes,
        )
    except Exception as e:
        raise RuntimeError("LDAP search failed") from e

    if len(connection.entries) == 0:
        raise ValueError("No matching entries found")

    for attr in attributes:
        if attr in connection.entries[0]:
            return connection.entries[0][attr].value

    raise ValueError("None of the expected attributes were found")


def resolve_to_dn(
    connection: Connection,
    search_base: str,
    search_filter: str,
    attribute: str,
) -> str:
    """
    Resolves a search filter to a Distinguished Name (DN) based on the provided attribute.

    Args:
        connection (Connection): The LDAP connection object.
        search_base (str): The search base to use for the search.
        search_filter (str): The search filter to use for the search.
        attribute (str): The attribute to resolve to a DN.

    Returns:
        str: The DN resolved from the attribute.

    Raises:
        ValueError: If no matching entries are found or if the attribute is not found.
    """
    cache_key: str = f"{search_base}:{search_filter}:{attribute}"
    cached_dn: Optional[Any] = cache.read(key=cache_key)
    if cached_dn:
        return cached_dn

    try:
        connection.search(
            search_base=search_base,
            search_filter=search_filter,
            attributes=[attribute],
        )
    except Exception as e:
        raise RuntimeError("LDAP search failed") from e

    if len(connection.entries) == 0:
        raise ValueError("No matching entries found")

    if connection.entries:
        entry: Entry = connection.entries[0]
        dn = entry.entry_dn
        cache.write(key=cache_key, value=dn)
        return dn

    raise ValueError(f"The '{attribute}' attribute was not found")


def query_for_users_ldap(
    connection: Connection,
    role: str,
    search_query: str,
    page: int,
    limit: int,
) -> Sequence[User]:
    """
    Queries for users in LDAP based on role and search query, with pagination support.

    Args:
        connection (Connection): The LDAP connection object.
        role (str): The role to filter users by.
        search_query (str): The search query to filter users by username or full name.
        page (int): The page number for pagination.
        limit (int): The number of users to return per page.

    Returns:
        Sequence[User]: A sequence of User objects matching the search criteria.

    Raises:
        ValueError: If the role is invalid, or if page or limit are less than 1.
        NoLDAPResultsError: If no matching users are found in LDAP.
    """
    if role not in app_settings.settings.auth.scopes:
        raise ValueError("Invalid role")

    if limit < 1:
        raise ValueError("Limit must be at least 1")

    if page < 1:
        raise ValueError("Page must be at least 1")

    # Create a cache key based on the role and search query
    cache_key: str = f"{role}:{search_query}"

    cached_users: Optional[Sequence[User]] = cast(
        Optional[Sequence[User]], cache.read(key=cache_key)
    )
    if cached_users is not None:
        cached_users_to_return: Sequence[User] = cached_users[
            (page - 1) * limit : page * limit  # noqa: E203
        ]
        return cached_users_to_return

    # if we didn't get a hit on the cache, perform a new search and cache the results
    # We start by resolving the group DN based on the role
    group_dn: str = resolve_to_dn(
        connection=connection,
        search_base=app_settings.settings.auth.ldap_base_dn,
        search_filter=f"(cn={app_settings.settings.auth.scopes[role]})",
        attribute="distinguishedName",
    )

    # Perform the search for users in the specified group with a username or full name matching the search query
    connection.search(
        search_base=app_settings.settings.auth.ldap_base_dn,
        search_filter=f"(&(|(sAMAccountName=*{search_query}*)(displayName=*{search_query}*))(memberOf:1.2.840.113556.1.4.1941:={group_dn}))",
        search_scope=SUBTREE,
        attributes=["sAMAccountName", "displayName", "objectGUID"],
    )

    if len(connection.entries) == 0:
        raise NoLDAPResultsError

    users: List[User] = []
    entry: UserSearchEntry
    for entry in connection.entries:
        users.append(
            User(
                id=hash_string(string=entry.objectGUID.value),
                user_name=entry.sAMAccountName.value,
                full_name=entry.displayName.value,
                role=role,
            )
        )

    cache.write(key=cache_key, value=users)

    users_to_return: Sequence[User] = users[
        (page - 1) * limit : page * limit  # noqa: E203
    ]

    return users_to_return
