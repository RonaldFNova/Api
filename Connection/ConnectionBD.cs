namespace API.Connection
{
    public class ConnectionBD
    {
        private readonly string Conexion;
        public ConnectionBD()
        {
            Conexion = Environment.GetEnvironmentVariable("DefaultConnection");

            if (string.IsNullOrEmpty(Conexion)) throw new Exception("No se ha configurado DefaultConnection.");
        }
        public String ConnectionMYSQL()
        {
            return Conexion;
        }
    }
}
