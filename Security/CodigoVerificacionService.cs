namespace API.Security
{
    public class CodigoVerificacionService
    {
        public string GenerarCodigo()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
