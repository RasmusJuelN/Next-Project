
namespace API.Exceptions;

/// <summary>
/// Represents an exception that contains an HTTP status code and message to be returned in an HTTP response.
/// </summary>
/// <param name="statusCode">The HTTP status code to be returned with the exception.</param>
/// <param name="message">The error message describing the exception.</param>
public class HttpResponseException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}