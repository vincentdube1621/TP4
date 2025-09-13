using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace BibliothequeLIPAJOLI.Services
{
    /// <summary>
    /// Middleware pour la gestion globale des exceptions
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une exception non gérée s'est produite");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidOperationException => (int)HttpStatusCode.Conflict,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                error = new
                {
                    message = exception.Message,
                    statusCode = context.Response.StatusCode,
                    timestamp = DateTime.UtcNow
                }
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Extensions pour enregistrer le middleware
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
