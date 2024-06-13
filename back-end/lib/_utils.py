"""
Utility functions for the back-end.
"""

from ldap3 import Connection  # type: ignore
from typing import List


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
