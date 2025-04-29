using CurrencyMicroService.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyMicroService.SharedModule
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

            // register custom exceptions

            (string Detail, string Title, int StatusCode) details = exception switch
            {
                NotFoundException notFoundEx =>
                (
                    notFoundEx.Message,
                    "Resource Not Found",
                    StatusCodes.Status404NotFound
                ),
                BadRequestException badRequestEx =>
                (
                    badRequestEx.Message,
                    "Bad Request",
                    StatusCodes.Status400BadRequest
                ),
                InternalServerException internalEx =>
                (
                    internalEx.Message,
                    "Internal Server Error",
                    StatusCodes.Status500InternalServerError
                ),
                _ =>
                (
                    "An unexpected error occurred",
                    "Internal Server Error",
                    StatusCodes.Status500InternalServerError
                )
            };

            var problemDetails = new ProblemDetails
            {
                Title = details.Title,
                Detail = details.Detail,
                Status = details.StatusCode,
                Instance = context.Request.Path
            };

            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

            context.Response.StatusCode = details.StatusCode;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
