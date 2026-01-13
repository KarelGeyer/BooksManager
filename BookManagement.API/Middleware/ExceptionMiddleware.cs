using System.Net;
using System.Text.Json;
using BookManagement.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BookManagement.Common.Middleware
{
    /// <summary>
    /// Middleware responsible for handling unhandled exceptions and converting them
    /// into standardized HTTP error responses.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware and catches any unhandled exceptions
        /// that occur during request processing.
        /// </summary>
        /// <param name="context">
        /// The current HTTP context.
        /// </param>
        /// <returns>
        /// A task that represents the completion of request processing.
        /// </returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions by mapping them to appropriate HTTP status codes
        /// and returning a JSON error response.
        /// </summary>
        /// <param name="context">
        /// The current HTTP context.
        /// </param>
        /// <param name="exception">
        /// The exception that was thrown.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous exception handling operation.
        /// </returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case FailedToCreateException:
                    status = HttpStatusCode.Conflict;
                    message = exception.Message;
                    break;
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                case FailedToLendExpection:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            _logger.LogError(exception, "Unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var response = new { error = message };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
