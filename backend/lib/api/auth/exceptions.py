class MissingSecretKeyError(Exception):
    """
    Exception raised when the secret key is missing in the settings file.

    This exception is used to indicate that the `auth.secret_key` must be set in the settings file.
    The secret key is essential for signing and verifying JWT tokens. It should be a long, random string.
    A suitable key can be generated using the command `openssl rand -hex 32`.

    Attributes:
        message (str): Explanation of the error.
    """

    def __init__(self) -> None:
        super().__init__(
            "A secret key in the settings file under `auth.secret_key` must be set. It is used to sign and verify JWT tokens. It should be a long, random string. One can be generated with `openssl rand -hex 32`."
        )


class MissingADServiceAccountError(Exception):
    """
    Exception raised when the Active Directory service account credentials are missing.

    This exception is triggered when the settings file does not contain the required
    Active Directory service account username and password under `auth.ad_service_account`
    and `auth.ad_service_password`. These credentials are necessary for querying the
    LDAP server for user information.

    Attributes:
        message (str): Explanation of the error.
    """

    def __init__(self) -> None:
        super().__init__(
            "An Active Directory service account username and password must be set in the settings file under `auth.ad_service_account` and `auth.ad_service_password`. This account is used to query the LDAP server for user information."
        )


class NoLDAPResultsError(Exception):
    """
    Exception raised when no results are returned from an LDAP query.

    This exception is raised when an LDAP query returns no results. This could be due to
    the query being invalid, or the LDAP server not returning any results for the given query.

    Attributes:
        message (str): Explanation of the error.
    """

    def __init__(self) -> None:
        super().__init__("No results were returned from the LDAP query.")


class MissingSaltHashError(Exception):
    """
    Exception raised when the salt hash is missing in the settings file.

    This exception is used to indicate that the `auth.salt_hash` must be set in the settings file.
    The salt hash is essential for hashing user data. It should be a long, random string.
    A suitable key can be generated using the command `openssl rand -hex 32`.

    Attributes:
        message (str): Explanation of the error.
    """

    def __init__(self) -> None:
        super().__init__(
            "A salt hash in the settings file under `auth.salt_hash` must be set. It is used to hash user data. It should be a long, random string. One can be generated with `openssl rand -hex 32`."
        )
