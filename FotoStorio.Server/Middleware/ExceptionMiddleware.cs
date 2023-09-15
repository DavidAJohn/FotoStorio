using FotoStorio.Server.Helpers;
using System.Net;
using System.Text.Json;

namespace FotoStorio.Server.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            _logger.LogError("Exception: {error}, Message: {message}", error, error.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = error switch
            {
                AppException => (int)HttpStatusCode.BadRequest, // custom application error
                _ => (int)HttpStatusCode.InternalServerError, // unhandled error
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var result = JsonSerializer.Serialize(new { message = error?.Message }, options);

            await response.WriteAsync(result);
        }
    }
}
