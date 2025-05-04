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

    public async Task GetTokenVerificarAsync(ModelTokenVerificar modelo)
    {
        if (string.IsNullOrEmpty(modelo.token))
            throw new ArgumentException("Token vacío.");

        string? userId = _tokenHelper.ObtenerUserIdDesdeTokenValidado(modelo.token);

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido.");
    }
}

}