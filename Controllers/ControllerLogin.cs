using Microsoft.AspNetCore.Mvc;
using API.Model;
using API.Data;
using API.Security;
using Microsoft.AspNetCore.Authorization;

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
        
        public async Task<IActionResult> Login([FromBody] ModelLogin loginRequest)
        {
            var user = await _dataLogin.GetUserByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Usuario incorrecto (user no encontrado)" });
            }

            bool isPasswordValid = _passwordHasher.VerifyPassword(loginRequest.Pass, user.Pass);
            if (!isPasswordValid)
            {
                return BadRequest(new { message = "contraseña incorrectos (pass no válido)" });
            }

            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Tipo);

            await _dataLogin.InsertLoginAsync(user.Id);

            return Ok(new { mensaje = "Código reenviado correctamente", token, user_type = user.Tipo });
        }
    }
}
