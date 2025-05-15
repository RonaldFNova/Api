using API.Model;
using API.Security;

namespace API.Data 
{
    public class DataTokenVerificar
    {
        private readonly TokenHelper _tokenHelper;

        public DataTokenVerificar(TokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
        }

        public Task GetTokenVerificarId(ModelTokenVerificar modelo)
        {

            string? userId = _tokenHelper.ObtenerUserIdDesdeTokenValidado(modelo.token);

            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("Token inválido.");
            
            return Task.CompletedTask;
        }

        public Task GetTokenVerificarCodigo(ModelTokenVerificar modelo)
        {

            (string? userId, string? codigo) = _tokenHelper.ObtenerUserIdCodigoDesdeTokenValidado(modelo.token);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(codigo)) throw new UnauthorizedAccessException("Token inválido.");

            return Task.CompletedTask;
        }
    }
}