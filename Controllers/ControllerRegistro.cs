using API.Data;
using API.Model;
using API.Security; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Registro")]
    public class ControllerRegistro : ControllerBase
    {

        private readonly DataRegistro _dataRegistro;
        private readonly JwtService _jwtService;

        public ControllerRegistro(DataRegistro dataRegistro, JwtService jwtService)
        {
            _dataRegistro = dataRegistro;
            _jwtService = jwtService;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> POST([FromBody] ModelRegistro parametros)
        {
            int id = await _dataRegistro.InsertarUsuario(parametros);
            var token = _jwtService.GenerateToken(id.ToString(), parametros.Tipo);
            return Ok(new { mensaje = "Registro enviado correctamente", token });
        }

        [HttpGet]
        [Authorize]

        public async Task<ActionResult<List<ModelRegistro>>> GET()
        {
            var lista = await _dataRegistro.MostrarUsuario();
            return lista;
        }

        [HttpPut("{id}")]
        [Authorize]

        public async Task<ActionResult> PUT(int id, [FromBody] ModelRegistro parametros)
        {
            parametros.Id = id;
            await _dataRegistro.EditarUsuario(parametros);
            return NoContent();
        }
    }
}


