using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using MySql.Data.MySqlClient; 
using API.Error;
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

            case UsuarioNoEncontradoException:
                statusCode = HttpStatusCode.NotFound;
                mensajeError = exception.Message;
                break;

            case CodigoIncorrectoException:
                statusCode = HttpStatusCode.BadRequest;
                mensajeError = exception.Message;
                break;

            case EmailNoEncontradoException:
                statusCode = HttpStatusCode.BadRequest;
                mensajeError = exception.Message;
                break;

            case EstadoUsuarioVerificadoException:
                statusCode = HttpStatusCode.BadRequest;
                mensajeError = exception.Message;
                break;

            case EstadoEmailVerificadoException:
                statusCode = HttpStatusCode.BadRequest;
                mensajeError = exception.Message;
                break;

            case TokenExpiradoException:
                statusCode = HttpStatusCode.Unauthorized;
                mensajeError = exception.Message;
                break;          

            case TokenInvalidoException:
                statusCode = HttpStatusCode.Unauthorized;
                mensajeError = exception.Message;
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
