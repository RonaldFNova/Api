using API.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Error;
using API.Data;
using API.Middleware;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
 

string secretKeyPath = "/etc/secrets/JwtSecretKey";
string _secretKey;

if (File.Exists(secretKeyPath)) _secretKey = File.ReadAllText(secretKeyPath).Trim();

else
{        
    Env.Load();
    _secretKey = Environment.GetEnvironmentVariable("JwtSecretKey") ?? string.Empty;
}

if (string.IsNullOrWhiteSpace(_secretKey)) throw new Exception("No se ha configurado JwtSecretKey.");


builder.Services.AddSingleton(new JwtService(_secretKey));

builder.Services.AddScoped<DataLogin>();

builder.Services.AddScoped<DataRegistro>();

builder.Services.AddScoped<DataTokenVerificar>();

builder.Services.AddScoped<CodigoVerificacionService>();

builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<TokenHelper>();

builder.Services.AddSingleton<PasswordHasher>();

builder.Services.AddControllers();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.ASCII.GetBytes(_secretKey);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(key),
        ValidateIssuer           = false,
        ValidateAudience         = false,
        ClockSkew                = TimeSpan.Zero
    };


    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                throw new TokenExpiradoException();
            }
            return Task.CompletedTask;
        }
    };
});


var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
