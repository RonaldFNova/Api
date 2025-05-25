using Microsoft.AspNetCore.Mvc;
using API.Model;
using API.Data;
using API.Security;
using Microsoft.AspNetCore.Authorization;
using Org.BouncyCastle.Pkcs;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Login")]
    public class ControllerLogin : ControllerBase
    {
        private readonly DataLogin _dataLogin;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher _passwordHasher;

        public ControllerLogin(DataLogin dataLogin, JwtService jwtService, PasswordHasher passwordHasher)
        {
            _dataLogin = dataLogin;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> POST([FromBody] ModelLogin parametros)
        {
            var user = await _dataLogin.GetUserByEmailAsync(parametros.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Usuario incorrecto (user no encontrado)" });
            }

            bool isPasswordValid = _passwordHasher.VerifyPassword(parametros.Pass, user.Pass);
            if (!isPasswordValid)
            {
                return BadRequest(new { message = "contraseña incorrectos (pass no válido)" });
            }

            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Tipo);

            await _dataLogin.InsertLoginAsync(user.Id);

            return Ok(new { mensaje = "Código reenviado correctamente", token, user_type = user.Tipo });
        }


        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GET([FromBody] ModelLogin parametros)
        {
            var lista = await _dataLogin.MostrarLoginAsync(parametros);


            var listaLimpia = lista.Select(item =>
    {
        var limpio = new Dictionary<string, object>();

        foreach (var prop in typeof(ModelLogin).GetProperties())
        {
            var valor = prop.GetValue(item);

            if (valor is string s && !string.IsNullOrWhiteSpace(s))
                limpio[prop.Name] = s;
            else if (valor is int i && i != 0)
                limpio[prop.Name] = i;
            else if (valor is DateTime d && d != DateTime.MinValue)
                limpio[prop.Name] = d;
            else if (valor is not null && !(valor is string))
                limpio[prop.Name] = valor;
        }

        return limpio;
        });

            return Ok(new { mensaje = "Lista de login enviada correctamente", lista = listaLimpia });
        }
    }
}
