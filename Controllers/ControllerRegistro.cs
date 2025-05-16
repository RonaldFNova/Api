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

        public ControllerRegistro(DataRegistro dataRegistro,JwtService jwtService)
        {
            _dataRegistro = dataRegistro;
            _jwtService = jwtService;
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
            int id =  await _dataRegistro.InsertarUsuario(parametros);
            var token = _jwtService.GenerateToken(id.ToString());
            return Ok(new {mensaje = "Registro enviado correctamente", token});
        }

        [HttpPut("{id}")]
        [Authorize]

        public async Task<ActionResult> Put(int id, [FromBody] ModelRegistro parametros)
        {
            parametros.Id = id;
            await _dataRegistro.EditarUsuario(parametros);
            return NoContent();
        }
    }


    [ApiController]
    [Route("Api/Registro/Enviar-codigo")]
    public class ControllerEnviarCodigo: ControllerBase
    {   
        private readonly DataRegistro _dataRegistro;
        private readonly JwtService _jwtService;
        public ControllerEnviarCodigo(DataRegistro dataRegistro,JwtService jwtService)
        {
            _jwtService = jwtService;
            _dataRegistro = dataRegistro;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Post([FromBody] ModelEnviarCodigo parametros)
        {
            (string id, string codigo) = await _dataRegistro.EnviarCodigo(parametros);

            var tokenCodigo = _jwtService.GenerateVerificationToken(id,codigo);

            return Ok(new {mensaje = "Código enviado correctamente",tokenCodigo});
        }
    }


    [ApiController]
    [Route("Api/Registro/Confirmar-codigo")]

    public class ControllerConfirmar: ControllerBase
    {
        private readonly DataRegistro _dataRegistro;

        public ControllerConfirmar(DataRegistro dataRegistro)
        {
            _dataRegistro = dataRegistro;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Post([FromBody] ModelConfirmacion parametros)
        {

            await _dataRegistro.ConfirmarVerificacion(parametros);

            return Ok(new { mensaje = "Código confirmado correctamente" });

        }
    }
}


