using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SharedKernel.Domain.Exceptions;

namespace App.Exceptions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        GlobalExceptionLog.ExceptionError(logger, exception);

        httpContext.Response.StatusCode = exception switch
        {
            DomainValidationException => StatusCodes.Status400BadRequest,
            ValidationException => StatusCodes.Status400BadRequest,
            InvalidCredentialException => StatusCodes.Status400BadRequest,
            BusinessRuleValidationException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError,
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = ReasonPhrases.GetReasonPhrase(httpContext.Response.StatusCode),
                Detail = environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
            }
        });
    }
}
