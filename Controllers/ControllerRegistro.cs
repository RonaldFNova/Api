using API.Data;
using API.Model;
using API.Security; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Security;
using MySql.Data.MySqlClient;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Registro")]
    public class ControllerRegistro : ControllerBase
    {

        private readonly DataRegistro _dataRegistro;
        private readonly JwtService _jwtService;



        public ControllerRegistro(
            DataRegistro dataRegistro,
            JwtService jwtService
           )
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
            DateTime fecha_creacion = await _dataRegistro.InsertarUsuario(parametros);

            var token = _jwtService.GenerateToken(parametros.id.ToString());

            return Ok(new {_dataRegistro.userId, token, fecha_creacion });
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


    [ApiController]
    [Route("Api/Registro/Reenviar-codigo")]
    public class ControllerReenviar: ControllerBase
    {   
         private readonly DataRegistro _dataRegistro;

        public ControllerReenviar(DataRegistro dataRegistro)
        {
            _dataRegistro = dataRegistro;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Post([FromBody] ModelReenviar parametros)
        {
            DateTime fecha_creacion = await _dataRegistro.Reenviar(parametros);

            return Ok(new {fecha_creacion});
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

            return NoContent();                
        }
    }
}


