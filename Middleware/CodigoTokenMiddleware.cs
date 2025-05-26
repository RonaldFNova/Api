using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Middleware
{
    public class CodigoTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;

        public CodigoTokenMiddleware(RequestDelegate next)
        {
            _next = next;
            _secretKey = Environment.GetEnvironmentVariable("JwtSecretKey")
                         ?? throw new Exception("No se encontró la variable de entorno 'JwtSecretKey'.");
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;

            var requiereCodigo = path != null && (
                path.Contains("/Confirmar-codigo")
            );

            if (requiereCodigo)
            {
                var token = context.Request.Headers["TokenCodigo"].FirstOrDefault();

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("TokenCodigo requerido.");
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
                    await context.Response.WriteAsync("TokenCodigo inválido: " + ex.Message);
                    return;
                }
            }

            await _next(context);
        }
    }
}
