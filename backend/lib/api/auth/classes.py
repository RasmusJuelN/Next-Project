from ldap3 import ALL, Connection, Server, DIGEST_MD5, NTLM, SASL
from ldap3.core.exceptions import LDAPException
from ldap3.protocol.rfc4512 import DsaInfo
from typing import Literal, Optional, Union, Callable, List, Tuple
from enum import Enum


from backend.lib.api.auth import logger
from backend.lib.api.auth.exceptions import (
    InvalidLDAPAuthenticationMethodError,
    LDAPSignedIntegrityProtectionNotSupportedError,
)
from backend import app_settings

# Define constants for authentication methods
AUTH_METHOD_SIMPLE = "simple"
AUTH_METHOD_SASL_DIGEST_MD5 = "sasl-digest-md5"
AUTH_METHOD_NTLM = "NTLM"

AD_SPECIFIC_OIDS: dict[str, str] = {
    "1.2.840.113556.1.4.800": "LDAP_CAP_ACTIVE_DIRECTORY_OID",
    "1.2.840.113556.1.4.1670": "LDAP_CAP_ACTIVE_DIRECTORY_V51_OID",
    "1.2.840.113556.1.4.1791": "LDAP_CAP_ACTIVE_DIRECTORY_LDAP_INTEG_OID",
    "1.2.840.113556.1.4.1935": "lDAP_CAP_ACTIVE_DIRECTORY_V61_OID",
    "1.2.840.113556.1.4.2080": "LDAP_CAP_ACTIVE_DIRECTORY_V61_R2_OID",
    "1.2.840.113556.1.4.2237": "LDAP_CAP_ACTIVE_DIRECTORY_W8_OID",
}


class DirectoryServiceFeature(Enum):
    """
    Enum for Active Directory specific OIDs.

    This enum provides a list of Active Directory specific OIDs that
    can be used to determine if the LDAP server is an
    Active Directory server, as well as its feature set.
    Refer to https://oidref.com/1.2.840.113556.1.4 for more information.

    Attributes:
        `ACTIVE_DIRECTORY` (str): The OID for Active Directory.
            If this OID is present in the supported features of the
            LDAP server, it is an Active Directory server.
        `ACTIVE_DIRECTORY_V51` (str): The OID for Active Directory v5.1.
            if this OID is present in the supported features of the
            LDAP server, it is an Active Directory v5.1 server or later.
        `ACTIVE_DIRECTORY_LDAP_INTEG` (str): The OID for Active Directory LDAP Integ.
            If this OID is present in the supported features of the
            LDAP server, it is an Active Directory server which supports
            LDAP integrity protection using signing.
        `ACTIVE_DIRECTORY_V61` (str): The OID for Active Directory v6.1.
            If this OID is present in the supported features of the
            LDAP server, it is an Active Directory v6.1 server or later.
        `ACTIVE_DIRECTORY_V61_R2` (str): The OID for Active Directory v6.1 R2.
            If this OID is present in the supported features of the
            LDAP server, it is an Active Directory v6.1 R2 server or later.
        `ACTIVE_DIRECTORY_W8` (str): The OID for Active Directory W8.
            If this OID is present in the supported features of the
            LDAP server, it is an Active Directory server running on
            Windows 8 or later.
    """

    ACTIVE_DIRECTORY = "1.2.840.113556.1.4.800"
    ACTIVE_DIRECTORY_V51 = "1.2.840.113556.1.4.1670"
    ACTIVE_DIRECTORY_LDAP_INTEG = "1.2.840.113556.1.4.1791"
    ACTIVE_DIRECTORY_V61 = "1.2.840.113556.1.4.1935"
    ACTIVE_DIRECTORY_V61_R2 = "1.2.840.113556.1.4.2080"
    ACTIVE_DIRECTORY_W8 = "1.2.840.113556.1.4.2237"


class LDAPConnection:
    """
    LDAPConnection class for handling LDAP authentication using various
    methods, including the option to provide a custom authentication
    method instead of the built-in methods.

    This class provides methods to authenticate a user against an LDAP
    server using SASL with DIGEST-MD5, simple authentication, NTLM,
    or a custom method. It also supports context management to
    ensure proper cleanup of the connection.

    Attributes:
        server (Server): The LDAP server instance.
        connection (Optional[Connection]): The LDAP connection object.
        username (str): The username for authentication.
        custom_authentication_method (Optional[Callable[[str, str, Server], Connection]]):

    Methods:
        `authenticate_using_sasl_digest_md5() -> Connection:`
            Authenticates a username and password using SASL with DIGEST-MD5 against
            an LDAP server.
        `authenticate_using_simple() -> Connection:`
            Authenticates a username and password using simple authentication against
            an LDAP server.
        `authenticate_using_ntlm() -> Connection:`
            Authenticates a username and password using NTLM authentication against
            an LDAP server.
        `authenticate() -> Connection:`
            Authenticates a username and password using SASL, simple, or NTLM
            authentication against an LDAP server, based on the authentication
            method specified in the settings.
    """

    def __init__(
        self,
        username: str,
        password: str,
        custom_authentication_method: Optional[
            Callable[[str, str, Server], Connection]
        ] = None,
    ) -> None:
        """
        Initializes the LDAPConnection with the given username and password, and an optional
        custom authentication method.

        Args:
            username (str): The username for authentication.
            password (str): The password for authentication.
            custom_authentication_method (Optional[Callable[[str, str, Server], Connection]]):
                A custom authentication method that takes a username, password, and Server
                instance as arguments and returns an LDAP connection object. Defaults to None.
        """
        self.server: Server = self._create_server()
        self.username: str = username
        self.password: str = password
        self.connection: Optional[Connection] = None
        self.custom_authentication_method: Optional[
            Callable[[str, str, Server], Connection]
        ] = custom_authentication_method
        self.server_info: DsaInfo = self._get_server_info()

    def authenticate_using_sasl_digest_md5(self) -> Connection:
        """
        Authenticates a username and password using SASL
        (Simple Authentication and Security Layer) with DIGEST-MD5 against
        an LDAP server.

        Returns an automatically bound LDAP connection object on successful
        authentication with the provided username and password, using SASL
        with DIGEST-MD5, and with signing enabled for integrity protection.

        Returns:
            Connection: An LDAP connection object on successful authentication.

        Raises:
            LDAPSignedIntegrityProtectionNotSupportedError: If the LDAP server
                does not advertise support for signed integrity protection.
            LDAPException: If the connection fails for any reason. Use
                specific exceptions for more granular error handling.

        Notes:
            The connection is NOT automatically unbound.
            `Connection.unbind()` should be called when done.
            Alternatively, `LDAPConnection` can be used as a context
            manager instead to automatically unbind the connection.
            SASL can be very strict about the format of the credentials
            and the domain name when signing is enabled. If any issues
        self.connection: Connection = Connection(matches the expected format
            `<uid>|<cn>@<domain>` and that the domain name is correct.
            If the LDAP server is an Active Directory server, the username
            should be in the format `<sAMAccountName>@<domain>`. If AD runs
            on a FQDN, ensure that the domain name is the FQDN. If an IP
            address is used, SASL may not work due to the lack of a domain
            name.
        """
        if not (
            DirectoryServiceFeature.ACTIVE_DIRECTORY_LDAP_INTEG.value
            in self.server_info.supported_features
        ):
            raise LDAPSignedIntegrityProtectionNotSupportedError

        connection: Connection = Connection(
            server=self.server,
            auto_bind=False,
            version=3,
            authentication=SASL,
            sasl_mechanism=DIGEST_MD5,
            sasl_credentials=(
                None,
                self._build_username(),
                self.password,
                None,
                "sign",
            ),
            raise_exceptions=True,
        )
        try:
            connection.bind()

        except LDAPException as bind_error:
            connection.unbind()
            logger.error(msg=f"LDAP bind failed: {bind_error}")
            raise bind_error

        else:
            self.connection = connection
            return connection

    def authenticate_using_simple(self) -> Connection:
        """
        Authenticates a username and password using simple authentication
        against an LDAP server.

        Returns an automatically bound LDAP connection object on successful
        authentication with the provided username and password, using simple
        authentication.

        Returns:
            Connection: An LDAP connection object on successful authentication.

        Raises:
            LDAPException: If the connection fails for any reason. Use
                specific exceptions for more granular error handling.

        Notes:
            The connection is NOT automatically unbound.
            `Connection.unbind()` should be called when done.
            Alternatively, `LDAPConnection` can be used as a context
            manager instead to automatically unbind the connection.
            Simple authentication sends the password in plain text over the
            network. This is not secure and should be avoided if possible.
            Use SASL with DIGEST-MD5 for a more secure authentication method.
        """
        connection: Connection = Connection(
            server=self.server,
            auto_bind=False,
            version=3,
            user=self._build_username(),
            password=self.password,
            raise_exceptions=True,
        )
        try:
            connection.bind()

        except LDAPException as bind_error:
            connection.unbind()
            logger.error(msg=f"LDAP bind failed: {bind_error}")
            raise bind_error

        else:
            self.connection = connection
            return connection

    def authenticate_using_ntlm(self) -> Connection:
        """
        Authenticates a username and password using NTLM (NT LAN Manager)
        authentication against an LDAP server.

        Returns an automatically bound LDAP connection object on successful
        authentication with the provided username and password, using NTLM.

        Returns:
            Connection: An LDAP connection object on successful authentication.

        Raises:
            LDAPException: If the connection fails for any reason. Use
                specific exceptions for more granular error handling.

        Notes:
            The connection is NOT automatically unbound.
            `Connection.unbind()` should be called when done.
            Alternatively, `LDAPConnection` can be used as a context
            manager instead to automatically unbind the connection.
            NTLM is a challenge-response authentication protocol that is
            more secure than simple authentication. It is not as secure as
            SASL with DIGEST-MD5, but it is more secure than simple
            authentication. NTLM is widely used in Windows environments.
            Usernames are required to be in the pre-windows 2000 format
            `<domain>\<sAMAccountName>` where the domain is NOT dot
            separated. For example, `next.dev` should be `nextdev`.
        """
        if not self._is_active_directory():
            raise LDAPException("LDAP server is not an Active Directory server.")

        connection: Connection = Connection(
            server=self.server,
            auto_bind=False,
            version=3,
            user=self._build_username(),
            password=self.password,
            authentication=NTLM,
            raise_exceptions=True,
        )
        try:
            connection.bind()

        except LDAPException as bind_error:
            connection.unbind()
            logger.error(msg=f"LDAP bind failed: {bind_error}")
            raise bind_error

        else:
            self.connection = connection
            return connection

    def authenticate_using_anonymous(self) -> Connection:
        """
        Authenticates anonymously against an LDAP server.

        Returns an automatically bound LDAP connection object on a successful
        connection.

        Returns:
            Connection: An LDAP connection object on successful authentication.

        Raises:
            LDAPException: If the connection fails for any reason. Use
                specific exceptions for more granular error handling.

        Notes:
            The connection is NOT automatically unbound.
            `Connection.unbind()` should be called when done.
            Alternatively, `LDAPConnection` can be used as a context
            manager instead to automatically unbind the connection.
            Anonymous authentication often has limited access, if any, to
            the LDAP server. This should only be used to retrieve information
            advertised by the server, such as the supported features.
        """
        connection: Connection = Connection(
            server=self.server,
            auto_bind=False,
            version=3,
            raise_exceptions=False,  # For some reason, setting this to True causes ldap3 to raise "LdapErr: DSID-0C090728, comment: In order to perform this operation a successful bind must be completed on the connection"
        )
        try:
            connection.bind()

        except LDAPException as bind_error:
            connection.unbind()
            logger.error(msg=f"LDAP bind failed: {bind_error}")
            raise bind_error

        else:
            self.connection = connection
            return connection

    def authenticate(self) -> Connection:
        """
        Authenticates a username and password using SASL, simple, or NTLM
        authentication against an LDAP server.

        Returns an automatically bound LDAP connection object on successful
        authentication with the provided username and password, using SASL,
        simple, or NTLM.

        Returns:
            Connection: An LDAP connection object on successful authentication.

        Raises:
            LDAPException: If the connection fails for any reason. Use
                specific exceptions for more granular error handling.

        Notes:
            The connection is NOT automatically unbound.
            `Connection.unbind()` should be called when done.
            Alternatively, `LDAPConnection` can be used as a context
            manager instead to automatically unbind the connection.
            Delegates the authentication to the appropriate method based on
            the authentication method specified in the settings. The default
            authentication method is simple. Refer to each authentication
            method for more information on how it works and its security
            implications.

        """
        try:
            if self.custom_authentication_method is not None:
                connection: Connection = self.custom_authentication_method(
                    self.username, self.password, self.server
                )
            elif app_settings.settings.auth.authentication_method == AUTH_METHOD_SIMPLE:
                connection = self.authenticate_using_simple()
            elif (
                app_settings.settings.auth.authentication_method
                == AUTH_METHOD_SASL_DIGEST_MD5
            ):
                connection = self.authenticate_using_sasl_digest_md5()
            elif app_settings.settings.auth.authentication_method == AUTH_METHOD_NTLM:
                connection = self.authenticate_using_ntlm()
            else:
                raise InvalidLDAPAuthenticationMethodError
        except Exception as exception:
            raise exception
        else:
            return connection

    def _create_server(self) -> Server:
        """
        Creates and returns an LDAP server instance.

        This method retrieves the LDAP server host from the application settings
        and initializes a Server object with the specified host and information level.

        Returns:
            Server: An instance of the Server class configured with the LDAP server host.
        """
        host: str = app_settings.settings.auth.ldap_server

        return Server(
            host=host,
            get_info=ALL,
        )

    def _build_username(self) -> str:
        """
        Constructs a username based on the authentication method specified in the application settings.

        Returns:
            str: The constructed username.

        Raises:
            InvalidLDAPAuthenticationMethodError: If the authentication method is not recognized.
        """
        auth_method: Union[
            Literal["simple"], Literal["sasl-digest-md5"], Literal["NTLM"]
        ] = app_settings.settings.auth.authentication_method

        if auth_method == AUTH_METHOD_SIMPLE:
            return self._build_simple_username()
        elif auth_method == AUTH_METHOD_SASL_DIGEST_MD5:
            return self._build_sasl_digest_md5_username()
        elif auth_method == AUTH_METHOD_NTLM:
            return self._build_ntlm_username()
        else:
            raise InvalidLDAPAuthenticationMethodError

    def _build_simple_username(self) -> str:
        """
        Constructs a username for simple authentication.

        Returns:
            str: The constructed username.
        """
        return self.username

    def _build_sasl_digest_md5_username(self) -> str:
        """
        Constructs a username for SASL DIGEST-MD5 authentication.

        Returns:
            str: The constructed username.
        """
        return f"{self.username}@{app_settings.settings.auth.domain}"

    def _build_ntlm_username(self) -> str:
        """
        Constructs a username for NTLM authentication.

        Returns:
            str: The constructed username.
        """
        domain: str = app_settings.settings.auth.domain.replace(".", "")
        return f"{domain}\\{self.username}"

    def _is_active_directory(self) -> bool:
        """
        Determines if the LDAP server is an Active Directory server.

        Returns:
            bool: True if the LDAP server is an Active Directory server, False otherwise.
        """
        supported_features: List[Tuple[str, str, str, str]] = (
            self.server_info.supported_features
        )
        return any(
            DirectoryServiceFeature.ACTIVE_DIRECTORY.value in feature
            for feature in supported_features
        )

    def _get_server_info(self) -> DsaInfo:
        connection: Connection = self.authenticate_using_anonymous()
        server_info: DsaInfo = connection.server.info
        connection.unbind()
        return server_info

    def __enter__(self) -> Connection:
        """
        Establishes and returns a connection upon entering the runtime context.

        Returns:
            Connection: The authenticated connection object.
        """
        self.connection = self.authenticate()
        return self.connection

    def __exit__(self, exc_type, exc_value, traceback) -> None:
        """
        Handles the cleanup when exiting the context manager.

        This method is called when the context manager is exited. It ensures that
        the connection is properly unbound and set to None if it exists.

        Args:
            exc_type (type): The exception type, if an exception was raised.
            exc_value (Exception): The exception instance, if an exception was raised.
            traceback (traceback): The traceback object, if an exception was raised.

        Returns:
            None
        """
        if self.connection is None:
            return

        self.connection.unbind()
