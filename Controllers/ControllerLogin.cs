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
            try
            {
                var user = await _dataLogin.GetUserByEmailAsync(loginRequest.email);
                if (user == null)
                {
                    return BadRequest(new { message = "Usuario incorrecto (user no encontrado)" });
                }

                bool isPasswordValid = _passwordHasher.VerifyPassword(loginRequest.pass, user.pass);
                if (!isPasswordValid)
                {
                    return BadRequest(new { message = "contraseña incorrectos (pass no válido)" });
                }

                var token = _jwtService.GenerateToken(user.id.ToString());

                await _dataLogin.InsertLoginAsync(user.id);

                return Ok(new
                {
                    token,
                    user_id = user.id,
                    full_name = user.name,
                    user_type = user.tipo
                   
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
