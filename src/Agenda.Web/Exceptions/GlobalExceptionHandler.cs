using Agenda.Application.Exceptions;
using Agenda.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Web.Exceptions;




public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError( exception, "Error Message: {Message}, Occurred at: {Time}", exception.Message, DateTime.UtcNow);

        int statusCode;
        string title;
        string detail;
        switch (exception)
        {
            case BadHttpRequestException badReq:
                statusCode = StatusCodes.Status400BadRequest;
                title = "Bad Request";
                detail = badReq.Message;
                break;
            case NotFoundException notFound:
                statusCode = StatusCodes.Status404NotFound;
                title = "Not Found";
                detail = notFound.Message;
                break;
            case DomainException domainException:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                title = "Domain Exception";
                detail = domainException.Message;
                break;
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                title = "Server Error";
                detail = "An Error Occured on the server";
                break;

        }
        ProblemDetails problemDetails = new()
        {
            Title = title,
            Detail = detail,
            Status = statusCode

        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}