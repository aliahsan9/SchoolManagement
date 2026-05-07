using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace SchoolManagement.API.ExceptionHandling
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = httpContext.TraceIdentifier;

            if (exception is ValidationException vex)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                var errors = vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    traceId,
                    message = "Validation failed.",
                    errors
                }, cancellationToken);
                return true;
            }

            if (exception is UnauthorizedAccessException uex)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    traceId,
                    message = uex.Message
                }, cancellationToken);
                return true;
            }

            if (exception is InvalidOperationException iex)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    traceId,
                    message = iex.Message
                }, cancellationToken);
                return true;
            }

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                success = false,
                traceId,
                message = "An unexpected error occurred."
            }, cancellationToken);
            return true;
        }
    }
}