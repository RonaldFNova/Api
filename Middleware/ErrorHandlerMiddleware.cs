using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using MySql.Data.MySqlClient; 

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var mensajeError = "Ocurrió un error inesperado.";

        switch (exception)
        {
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                mensajeError = "Operación inválida.";
                break;

            case MySqlException:
                statusCode = HttpStatusCode.InternalServerError;
                mensajeError = "Error en la base de datos.";
                break;

            case ArgumentNullException:
                statusCode = HttpStatusCode.BadRequest;
                mensajeError = "Faltan datos obligatorios.";
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                mensajeError = "Acceso no autorizado.";
                break;

            default:
                break;
        }

        response.StatusCode = (int)statusCode;
        var result = JsonSerializer.Serialize(new
        {
            error = mensajeError
        });

        return response.WriteAsync(result);
    }
}
