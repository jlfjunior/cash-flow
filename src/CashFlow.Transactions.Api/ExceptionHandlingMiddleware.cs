using System.Net;
using CashFlow.Transactions.Domain.Exceptions;

namespace CashFlow.Transactions.Api;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                Message = ex.Message,
            });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                Message = ex.Message,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                Message = "An unexpected error occurred. Please try again later.",
            });
        }
    }
}