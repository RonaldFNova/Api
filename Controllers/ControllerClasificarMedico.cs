using Microsoft.AspNetCore.Mvc;
using API.Data;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Clasificar-medico")]

    public class ControllerClasificarMedico : ControllerBase
    {
        private readonly DataClasificarMedico _dataClasificarMedico;

        public ControllerClasificarMedico(DataClasificarMedico dataClasificarMedico)
        {
            _dataClasificarMedico = dataClasificarMedico;
        }


        [Authorize(Roles = "Paciente,Administrador")]
        [HttpPost]
        public async Task<IActionResult> POST([FromBody] ModelClasificarMedico parametros)
        {

            var medicos = await _dataClasificarMedico.ObtenerListaMedicoClasificarAsync(parametros);

            return Ok(new { mensaje = "La lista de medicos se obtuvo de manera correcta", medicos });
        }   

    }
}