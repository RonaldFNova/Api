using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text;

namespace API.Middleware
{
    public class CodigoTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;

        public CodigoTokenMiddleware(RequestDelegate next, string secretKey)
        {
            _next = next;
            _secretKey =  !string.IsNullOrWhiteSpace(secretKey)
                     ? secretKey
                     : throw new Exception("JwtSecretKey no está configurado.");
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;

            var requiereCodigo = path != null && (
                path.Contains("/Confirmar-codigo") ||
                path.Contains("/Confirmar-jwt-codigo") ||
                path.Contains("/Verificacion-sms/Recibir-codigo")
    

            );

            if (requiereCodigo)
            {
                var token = context.Request.Headers["TokenCodigo"].FirstOrDefault()?.Trim();

                if (string.IsNullOrWhiteSpace(token))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";

                    var error = new { error = "TokenCodigo requerido." };
                    var json = JsonSerializer.Serialize(error);

                    await context.Response.WriteAsync(json);
                    return;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                try
                {
                    var validationParams = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    var principal = tokenHandler.ValidateToken(token, validationParams, out _);

                    var userId = principal.FindFirst("id")?.Value;
                    var codigo = principal.FindFirst("codigo")?.Value;

                    context.Items["CodigoToken_UserId"] = userId;
                    context.Items["CodigoToken_Codigo"] = codigo;
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var error = new { error = "TokenCodigo inválido: " + ex.Message };
                    var json = JsonSerializer.Serialize(error);

                    await context.Response.WriteAsync(json);
                    return;
                }
            }

            await _next(context);
        }
    }
}
