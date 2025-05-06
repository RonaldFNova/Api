namespace API.Connection
{
    public class ConnectionBD
    {
        private readonly string Conexion;
        public ConnectionBD()
        {

            var ConexionPath = "/etc/secrets/Sendgrid_Api_Key";

            if (!File.Exists(ConexionPath))
                throw new Exception("No se ha configurado Sendgrid_Api_Key.");

            Conexion = File.ReadAllText(ConexionPath).Trim();
    
        }
        public String ConnectionMYSQL()
        {
            return Conexion;
        }
    }
}
