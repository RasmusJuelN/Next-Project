"""
Utility functions for the back-end.
"""

from ldap3 import Connection  # type: ignore
from typing import List
from uuid import UUID


SCOPES: dict[str, str] = {"student": "student", "teacher": "teacher", "admin": "admin"}


async def get_object_by_uuid(
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
            search_base="ou=next,dc=next,dc=dev",
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


async def determine_scope_from_groups(groups: List[str]) -> str:
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
        if group in SCOPES:
            return SCOPES[group]

    raise ValueError("No matching scopes found")


async def get_uuid_from_ldap(
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
            search_base="ou=next,dc=next,dc=dev",
            search_filter=f"(sAMAccountName={username})",
            attributes=["objectGUID"],
        )
    except Exception as e:
        raise RuntimeError("LDAP search failed") from e

    if len(connection.entries) == 0:
        raise ValueError("No matching entries found")

    if "objectGUID" in connection.entries[0]:
        uuid_str: str = connection.entries[0]["objectGUID"].value
        uuid_str = uuid_str.strip("{}")
        return str(object=UUID(hex=uuid_str))

    raise ValueError("The 'objectGUID' attribute was not found")


async def get_member_of_from_ldap(
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
            search_base="ou=next,dc=next,dc=dev",
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


async def get_full_name_from_ldap(
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
    return await return_first_non_empty_attribute(
        connection=connection,
        username=username,
        attributes=[
            "displayName",
            "cn",
            "name",
            "givenName",
        ],
    )


async def return_first_non_empty_attribute(
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
            search_base="ou=next,dc=next,dc=dev",
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
