using API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

/// <summary>
/// Action filter that converts HttpResponseException instances into proper HTTP responses.
/// </summary>
/// <remarks>
/// This filter intercepts exceptions of type HttpResponseException thrown during action execution
/// and converts them into appropriate HTTP responses with the status code and message specified in the exception.
/// This allows service layer code to throw HTTP-aware exceptions without coupling to ASP.NET Core types.
/// </remarks>
public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    /// <summary>
    /// Gets the order value for determining the sequence of filter execution.
    /// Higher values execute later in the pipeline.
    /// </summary>
    public int Order => int.MaxValue - 10;

    /// <summary>
    /// Called before the action method executes.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public void OnActionExecuting(ActionExecutingContext context) { }

    /// <summary>
    /// Called after the action method executes.
    /// Converts HttpResponseException into an HTTP response with appropriate status code.
    /// </summary>
    /// <param name="context">The action executed context.</param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is HttpResponseException httpResponseException)
        {
            context.Result = new ObjectResult(new { message = httpResponseException.Message })
            {
                StatusCode = (int)httpResponseException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}