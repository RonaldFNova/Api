using API.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Error;
using API.Data;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);
 
DotNetEnv.Env.Load("/etc/secrets/.env");
//DotNetEnv.Env.Load();

string secretKey = Environment.GetEnvironmentVariable("JwtSecretKey");

if (string.IsNullOrEmpty(secretKey))
    throw new Exception("No se ha configurado JwtSecretKey.");

builder.Services.AddSingleton(new JwtService(secretKey));

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
    var key = Encoding.ASCII.GetBytes(secretKey);
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
