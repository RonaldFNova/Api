using System.Net;
using System.Text.Json;
using MySql.Data.MySqlClient;
using API.Error;

namespace API.Middleware
{
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
                var customToken = context.Request.Headers["X-Codigo-Token"].FirstOrDefault();

                if (!string.IsNullOrEmpty(customToken))
                {
                    context.Items["TokenCodigo"] = customToken;
                }
                
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
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
                case CodigoIncorrectoException:
                case EmailNoEncontradoException:
                case EstadoUsuarioVerificadoException:
                case EstadoEmailVerificadoException:
                case TokenExpiradoException:
                case TokenInvalidoException:
                    statusCode = HttpStatusCode.BadRequest;
                    mensajeError = exception.Message;
                    break;

                default:
                    mensajeError = exception.Message;
                    break;
            }

            response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                error = mensajeError,
                exception = exception.Message,
                stackTrace = exception.StackTrace
            });

            return response.WriteAsync(result);
        }
    }
}
