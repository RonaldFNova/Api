using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Medico-fecha")]
    public class ControllerMedicoFecha : ControllerBase
    {
        private readonly DataMedicoFecha _dataMedicoFecha;


        public ControllerMedicoFecha(DataMedicoFecha dataMedicoFecha)
        {
            _dataMedicoFecha = dataMedicoFecha;
        }


        [HttpPost]
        [Authorize(Roles = "Paciente,Administrador")]
        public async Task<IActionResult> POST([FromBody] ModelMedicoFecha parametros)
        {
            int id = await _dataMedicoFecha.MostraFechaMedicaAsync(parametros);

            var listaFechas = await _dataMedicoFecha.ObternerFechasDisponiblesAsync(id);

            return Ok(listaFechas);
        }
    }
}