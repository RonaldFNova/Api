using API.Data;
using API.Model;
using API.Security; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Registro")]
    public class ControllerUsuario : ControllerBase
    {

        private readonly DataRegistro _dataRegistro;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher _passwordHasher;

        public ControllerUsuario(DataRegistro dataRegistro, JwtService jwtService, PasswordHasher passwordHasher)
        {
            _dataRegistro = dataRegistro;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        [Authorize]

        public async Task<ActionResult<List<ModelRegistro>>> Get()
        {
            var lista = await _dataRegistro.MostrarUsuario();
            return lista;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Post([FromBody] ModelRegistro parametros)
        {
            await _dataRegistro.InsertarUsuario(parametros);

            var token = _jwtService.GenerateToken(parametros.id.ToString());

            return Ok(new { token });
        }

        [HttpPut("{id}")]
        [Authorize]

        public async Task<ActionResult> Put(int id, [FromBody] ModelRegistro parametros)
        {
            parametros.id = id;
            await _dataRegistro.EditarUsuario(parametros);
            return NoContent();
        }
    }
}
