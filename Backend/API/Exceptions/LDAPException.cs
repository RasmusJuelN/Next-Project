namespace API.Exceptions;

public class LDAPException
{
    public class ConnectionErrorException : Exception
    {
        public ConnectionErrorException() {}
        public ConnectionErrorException(string message)
            : base(message) {}
        public ConnectionErrorException(string message, Exception inner)
            : base(message, inner) {}
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() {}
        public InvalidCredentialsException(string message)
            : base(message) {}
        public InvalidCredentialsException(string message, Exception inner)
            : base(message, inner) {}
    }
}
