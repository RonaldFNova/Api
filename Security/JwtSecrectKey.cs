namespace API.Securit
{
    public class JwtSecrectKey
    {
        private string secrectkeyString = string.Empty;
        public JwtSecrectKey()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            secrectkeyString = builder.GetSection("JwtSecretKey").Value;
        }

        public String SecrectKey()
        {
            return secrectkeyString;
        }

    }

}

