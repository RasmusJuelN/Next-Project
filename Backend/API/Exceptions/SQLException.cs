namespace API.Exceptions;

public class SQLException
{
    public class ItemAlreadyExists : Exception
    {
        public ItemAlreadyExists() {}
        public ItemAlreadyExists(string message)
            : base(message) {}
        public ItemAlreadyExists(string message, Exception inner)
            : base(message, inner) {} 
    }

    public class ItemNotFound : Exception
    {
        public ItemNotFound() {}
        public ItemNotFound(string message)
            : base(message) {}
        public ItemNotFound(string message, Exception inner)
            : base(message, inner) {}
    }
}
