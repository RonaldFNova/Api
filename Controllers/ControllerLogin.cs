using Microsoft.AspNetCore.Mvc;
using API.Model;
using API.Data;
using API.Security;
using Microsoft.AspNetCore.Authorization;

namespace TuProyectoAPI.Controllers
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
                    return BadRequest(new { message = "Usuario o contraseña incorrectos (user no encontrado)" });
                }

                bool isPasswordValid = _passwordHasher.VerifyPassword(loginRequest.pass, user.Pass);
                if (!isPasswordValid)
                {
                    return BadRequest(new { message = "Usuario o contraseña incorrectos (pass no válido)" });
                }

                var token = _jwtService.GenerateToken(user.user_id.ToString());

                await _dataLogin.InsertLoginAsync(user.user_id);

                return Ok(new
                {
                    token = token,
                    user_id = user.user_id,
                    full_name = user.full_name,
                    user_type = user.user_type
                   
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
