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


class InvalidLDAPAuthenticationMethodError(Exception):
    """
    Exception raised when an invalid LDAP authentication method is specified in the settings file.

    This exception is triggered when the `auth.authentication_method` setting in the settings file
    is not one of the supported LDAP authentication methods: "simple", "sasl-digest-md5", or "NTLM".

    Attributes:
        message (str): Explanation of the error.
    """

    def __init__(self) -> None:
        super().__init__(
            "The LDAP authentication method specified in the settings file under `auth.authentication_method` is invalid. Supported methods are 'simple', 'sasl-digest-md5', and 'NTLM'."
        )


class LDAPControlNotAdvertisedException(Exception):
    """
    Baseclass exception raised when the LDAP server does not advertise the requested extended control.

    This exception is raised when the LDAP server does not advertise one or all of the requested
    extended controls. This could be due to the LDAP server not supporting the requested control,
    or the control being disabled on the server.

    Attributes:
        message (str): Explanation of the error.
    """

    def __init__(self, control_oid: str) -> None:
        super().__init__(
            f"The LDAP server does not advertise the requested extended control with OID {control_oid}."
        )


class LDAPSignedIntegrityProtectionNotSupportedError(LDAPControlNotAdvertisedException):
    """
    Exception raised when the LDAP server does not advertise support for signed integrity protection.

    This exception is triggered when the LDAP server does not advertise support for signed integrity
    protection, which is required for the LDAP authentication method "sasl-digest-md5".

    Attributes:
        message (str): Explanation of the error.
    """

    def __init__(self) -> None:
        super().__init__(control_oid="1.2.840.113556.1.4.1791")
