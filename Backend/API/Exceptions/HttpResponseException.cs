using System.Net;

namespace API.Exceptions;

public class HttpResponseException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}