using Microsoft.AspNetCore.Http;

namespace Ibatech.API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado."),
            ArgumentException => (StatusCodes.Status400BadRequest, "Erro de validação."),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Não autorizado."),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflito de regra de negócio."),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor.")
        };

        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(new
        {
            message = message,
            detail = exception.Message
        });
    }
}
