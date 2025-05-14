using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using API.Error;
using DotNetEnv;

namespace API.Security
{   
    public class TokenHelper
    {
        private readonly string _secretKey;

        public TokenHelper()
        {

            string secretKeyPath = "/etc/secrets/JwtSecretKey";

            if (File.Exists(secretKeyPath)) _secretKey = File.ReadAllText(secretKeyPath).Trim();
            
            else
            {        
                Env.Load();
                _secretKey = Environment.GetEnvironmentVariable("JwtSecretKey") ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(_secretKey)) throw new Exception("No se ha configurado JwtSecretKey.");
        }

        public string? ObtenerUserIdDesdeTokenValidado(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "id");
                return userIdClaim?.Value;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new TokenExpiradoException();
            }
            catch (Exception)
            {
                throw new TokenInvalidoException();
            }
        }

        public (string? userId, string? codigo) ObtenerUserIdCodigoDesdeTokenValidado(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);


            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "id");
                var codigoClaim = principal.Claims.FirstOrDefault(c => c.Type == "codigo");
                return (userIdClaim?.Value, codigoClaim?.Value);
            }
            catch (SecurityTokenExpiredException)
            {
                throw new TokenExpiradoException();
            }
            catch (Exception)
            {
                throw new TokenInvalidoException();
            }
        }
    }
}
