namespace API.Connection
{
    public class ConnectionBD
    {
        private readonly string Conexion;
        public ConnectionBD()
        {

            var ConexionPath = "/etc/secrets/DefaultConnection";

            if (!File.Exists(ConexionPath))
                throw new Exception("No se ha configurado DefaultConnection.");

            Conexion = File.ReadAllText(ConexionPath).Trim();
    
        }
        public String ConnectionMYSQL()
        {
            return Conexion;
        }
    }
}
