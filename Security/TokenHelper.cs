using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using API.Error;

namespace API.Security
{   
    public class TokenHelper
    {
        private readonly string _secretKey;

        public TokenHelper(IConfiguration configuration)
        {
            _secretKey = configuration["JwtSecretKey"];
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
    }
}
