using API.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Data;
using API.Middleware;
using API.Connection;
using DotNetEnv;
using System.Text.Json;

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

builder.Services.AddScoped<DataMostrarDoctores>();

builder.Services.AddScoped<DataTipoUser>();

builder.Services.AddScoped<DataPatientPersonalInform>();

builder.Services.AddScoped<DataTokenVerificar>();

builder.Services.AddScoped<CodigoVerificacionService>();

builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<TokenHelper>();

builder.Services.AddSingleton<PasswordHasher>();

builder.Services.AddSingleton<ConnectionBD>();

builder.Services.AddControllers();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.ASCII.GetBytes(_secretKey);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };


    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new { mensaje = "El token ha expirado." });
                    return context.Response.WriteAsync(result);
                }
            }

            return Task.CompletedTask;
        }
    };

});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllLocalhost", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            origin.StartsWith("http://localhost") ||
            origin.StartsWith("https://localhost") ||
            origin.StartsWith("http://127.0.0.1") ||
            origin.StartsWith("https://127.0.0.1"))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

app.UseCors("AllowAllLocalhost");

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
