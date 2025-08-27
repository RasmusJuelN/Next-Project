namespace API.Exceptions;

/// <summary>
/// Contains custom exception classes for SQL-related operations.
/// </summary>
public class SQLException
{
    /// <summary>
    /// Exception thrown when attempting to create an item that already exists in the database.
    /// </summary>
    public class ItemAlreadyExists : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemAlreadyExists"/> class.
        /// </summary>
        public ItemAlreadyExists() {}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemAlreadyExists"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ItemAlreadyExists(string message)
            : base(message) {}
            
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemAlreadyExists"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public ItemAlreadyExists(string message, Exception inner)
            : base(message, inner) {} 
    }

    /// <summary>
    /// Exception thrown when attempting to access an item that does not exist in the database.
    /// </summary>
    public class ItemNotFound : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotFound"/> class.
        /// </summary>
        public ItemNotFound() {}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotFound"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ItemNotFound(string message)
            : base(message) {}
            
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotFound"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public ItemNotFound(string message, Exception inner)
            : base(message, inner) {}
    }
}
