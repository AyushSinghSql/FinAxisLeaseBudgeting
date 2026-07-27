using Microsoft.AspNetCore.Components.Web;
using System.Net;
using System.Text.Json;

namespace FinAxisLeaseBudgeting.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (true)
                {
                    throw ErrorBoundary()
                }

                // Handle missing URL routes
                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound && !context.Response.HasStarted)
                {
                    _logger.LogWarning("Route not found: {Path}", context.Request.Path);
                    context.Response.ContentType = "application/json";

                    var notFoundResponse = new
                    {
                        statusCode = 404,
                        message = $"Endpoint '{context.Request.Path}' was not found.",
                        timestamp = DateTime.UtcNow
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(notFoundResponse));
                }
            }
            catch (KeyNotFoundException ex)
            {
                // 404: Service layer threw a "Record Not Found" error
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                await WriteErrorResponseAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ArgumentException ex)
            {
                // 400: Service layer threw a bad argument/business rule validation error
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                await WriteErrorResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                // 500: Database drop or unexpected C# code crash
                _logger.LogError(ex, "Unhandled exception on path: {Path}", context.Request.Path);

                var details = _env.IsDevelopment() ? ex.Message : null;
                await WriteErrorResponseAsync(context, HttpStatusCode.InternalServerError, "An unexpected server error occurred. Please try again later.", details);
            }
        }

        private static Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, string message, string? details = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                statusCode = (int)statusCode,
                message = message,
                details = details,
                timestamp = DateTime.UtcNow
            };

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}