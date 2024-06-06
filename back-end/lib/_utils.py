from ldap3 import Connection, ALL_ATTRIBUTES  # type: ignore
from typing import List


async def get_full_name_from_ldap(connection: Connection, username: str) -> str:
    """
    Retrieves the full name of a user from LDAP based on their username.

    Args:
        connection (Connection): The LDAP connection object.
        username (str): The username of the user.

    Returns:
        str: The full name of the user.

    Raises:
        RuntimeError: If the LDAP search fails.
        ValueError: If no matching entries are found or if none of the expected attributes are found.
    """
    try:
        connection.search(
            search_base="ou=next,dc=next,dc=dev",
            search_filter=f"(sAMAccountName={username})",
            attributes=[ALL_ATTRIBUTES],
        )
    except Exception as e:
        raise RuntimeError("LDAP search failed") from e

    if len(connection.entries) == 0:
        raise ValueError("No matching entries found")

    attributes: List[str] = [
        "displayName",
        "cn",
        "name",
        "givenName",
        "distinguishedName",
    ]
    for attr in attributes:
        if attr in connection.entries[0]:
            if attr == "distinguishedName":
                fields: List[str] = connection.entries[0][attr].value.split(",")
                for field in fields:
                    if field.startswith("CN="):
                        return field.split("=")[1]
            else:
                return connection.entries[0][attr].value

    raise ValueError("None of the expected attributes were found")
