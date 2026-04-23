using PetRadar.API.Infrastructure.Responses;
using PetRadar.SharedKernel.Exceptions;
using System.Net;
using System.Text.Json;

namespace PetRadar.API.Infrastructure.Middleware;

internal sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception caught by global middleware");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message) = exception switch
        {
            UnauthorizedException ex => (HttpStatusCode.Unauthorized,          ex.ErrorCode, ex.Message),
            ForbiddenException ex    => (HttpStatusCode.Forbidden,             ex.ErrorCode, ex.Message),
            NotFoundException ex     => (HttpStatusCode.NotFound,              ex.ErrorCode, ex.Message),
            ConflictException ex     => (HttpStatusCode.Conflict,              ex.ErrorCode, ex.Message),
            ValidationException ex   => (HttpStatusCode.BadRequest,            ex.ErrorCode, ex.Message),
            DomainException ex       => (HttpStatusCode.UnprocessableEntity,   ex.ErrorCode, ex.Message),
            _                        => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var body = new ApiErrorResponse(new ApiError(errorCode, message));

        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
