namespace API.Exceptions;

public class LDAPException
{
    public class ConnectionError : Exception
    {
        public ConnectionError() {}
        public ConnectionError(string message)
            : base(message) {}
        public ConnectionError(string message, Exception inner)
            : base(message, inner) {}
    }

    public class InvalidCredentials : Exception
    {
        public InvalidCredentials() {}
        public InvalidCredentials(string message)
            : base(message) {}
        public InvalidCredentials(string message, Exception inner)
            : base(message, inner) {}
    }

    public class NotBound : Exception
    {
        public NotBound() {}
        public NotBound(string message)
            : base(message) {}
        public NotBound(string message, Exception inner)
            :   base(message, inner) {}
    }
}
