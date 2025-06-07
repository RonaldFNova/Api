using API.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Data;
using API.Middleware;
using API.Connection;
using DotNetEnv;
using API.Error;

var builder = WebApplication.CreateBuilder(args);


string secretKeyPath = "/etc/secrets/JwtSecretKey";
string _secretKey = "";


if (File.Exists(secretKeyPath))
{
    _secretKey = File.ReadAllText(secretKeyPath).Trim();
    Console.WriteLine($"Clave cargada desde archivo: {secretKeyPath}");
}
else
{
    Env.Load(Path.Combine(builder.Environment.ContentRootPath, ".env"));
    _secretKey = Environment.GetEnvironmentVariable("JwtSecretKey") ?? "";
    Console.WriteLine("Clave cargada desde variables de entorno");
}


if (string.IsNullOrWhiteSpace(_secretKey))
{
    throw new InvalidOperationException(
        "FALLA CRÍTICA: No se configuró JWT_SECRET_KEY. " +
        "Verifica:\n" +
        $"1. Archivo .env en {Path.Combine(builder.Environment.ContentRootPath, ".env")}\n" +
        $"2. Archivo secreto en {secretKeyPath}\n" +
        "3. Variables de entorno del sistema");
}

builder.Services.AddSingleton(_secretKey);

builder.Services.AddSingleton(new JwtService(_secretKey));

builder.Services.AddScoped<DataLogin>();

builder.Services.AddScoped<DataRegistro>();

builder.Services.AddScoped<DataHorarioMedico>();

builder.Services.AddScoped<DataClasificarMedico>();

builder.Services.AddScoped<DataMostrarMedicos>();

builder.Services.AddScoped<DataInsertarCitaMedicaId>();

builder.Services.AddScoped<DataTipoUser>();

builder.Services.AddScoped<DataInsertarCitaMedica>();

builder.Services.AddScoped<DataMedicoFecha>();

builder.Services.AddScoped<DataMostrarCitaFecha>();

builder.Services.AddScoped<DataListaEspecialidades>();

builder.Services.AddScoped<DataEnviarCodigoSms>();

builder.Services.AddScoped<DataConfirmarCodigoSms>();

builder.Services.AddScoped<DataProfesionalPersonalInform>();

builder.Services.AddScoped<DataPatientPersonalInform>();

builder.Services.AddScoped<CodigoVerificacionService>();

builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<SmsCodeVerificacionServices>();

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
                throw new TokenExpiradoException();
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

app.UseMiddleware<CodigoTokenMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();

