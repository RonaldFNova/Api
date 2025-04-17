namespace API.Connection
{
    public class ConnectionBD
    {
        private string conexionString = string.Empty;
        public ConnectionBD()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            conexionString = builder.GetSection("ConnectionStrings:CONEXIONPRUEBA").Value;
        }

        public String ConnectionMYSQL()
        {
            return conexionString;
        }

    }

}
