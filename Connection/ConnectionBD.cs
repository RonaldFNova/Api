using DotNetEnv;
namespace API.Connection

{
    public class ConnectionBD
    {
        private readonly string _Conexion;
        public ConnectionBD()
        { 

            string ConexionPath = "/etc/secrets/DefaultConnection";

            if (File.Exists(ConexionPath)) _Conexion = File.ReadAllText(ConexionPath).Trim();
            
            else
            {        
                Env.Load();
                _Conexion = Environment.GetEnvironmentVariable("DefaultConnection") ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(_Conexion)) throw new Exception("No se ha configurado DefaultConnection.");

        }
        public String ConnectionMYSQL()
        {
            return _Conexion;
        }
    }
}
