namespace API.Exceptions;

/// <summary>
/// Contains LDAP-specific exception classes for handling various LDAP operation errors.
/// </summary>
public class LDAPException
{
    /// <summary>
    /// Exception thrown when there is a connection error to the LDAP server.
    /// </summary>
    public class ConnectionError : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionError"/> class.
        /// </summary>
        public ConnectionError() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionError"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConnectionError(string message)
            : base(message) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionError"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public ConnectionError(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>
    /// Exception thrown when authentication fails due to invalid credentials.
    /// </summary>
    public class InvalidCredentials : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCredentials"/> class.
        /// </summary>
        public InvalidCredentials() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCredentials"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidCredentials(string message)
            : base(message) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCredentials"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public InvalidCredentials(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>
    /// Exception thrown when an LDAP operation is attempted without being properly bound to the LDAP server.
    /// </summary>
    public class NotBound : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotBound"/> class.
        /// </summary>
        public NotBound() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NotBound"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NotBound(string message)
            : base(message) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NotBound"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public NotBound(string message, Exception inner)
            : base(message, inner) { }
    }
}
