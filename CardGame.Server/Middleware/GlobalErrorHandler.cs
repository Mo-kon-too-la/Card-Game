using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CardGame.Server.Middleware;

public class GlobalErrorHandler
{
    private readonly ILogger<GlobalErrorHandler> _logger;
    private readonly RequestDelegate _next;

    public GlobalErrorHandler(ILogger<GlobalErrorHandler> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");

            var details = new ProblemDetails
            {
                Detail = ex.Message,
                Instance = context.Request.Path,
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected Internal Server Error",
                Type = "Error",
            };

            var response = JsonSerializer.Serialize(details);

            context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(response);

        }
    }
}
