using API.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Security;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string secretKey = builder.Configuration["JwtSecretKey"]
                   ?? throw new Exception("No se ha configurado JwtSecretKey.");

builder.Services.AddSingleton(new JwtService(secretKey));
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddScoped<API.Data.DataLogin>();
builder.Services.AddScoped<API.Data.DataRegistro>();

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
});

builder.Services.AddScoped<CodigoVerificacionService>();

builder.Services.AddScoped<EmailService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();
