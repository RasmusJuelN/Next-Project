from ldap3.abstract.entry import Entry
from ldap3.abstract.attribute import Attribute


class UserSearchEntry(Entry):
    """
    UserSearchEntry class is a subclass of the Entry class from the ldap3 library.

    This class is used to represent a user search result from the LDAP server.
    """

    displayName: Attribute
    objectGUID: Attribute
    sAMAccountName: Attribute
