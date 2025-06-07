using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Mostrar-citas-fecha")]

    public class ControllerMostrarCitaFecha : ControllerBase
    {
        private readonly DataMostrarCitaFecha _dataMostrarCitaFecha;

        public ControllerMostrarCitaFecha(DataMostrarCitaFecha dataMostrarCitaFecha)
        {
            _dataMostrarCitaFecha = dataMostrarCitaFecha;
        }

        [Authorize(Roles = "Paciente")]
        [HttpPost]
        public async Task<IActionResult> POST([FromBody] ModelMostrarCitaFecha parametros)
        {
            var Lista = await _dataMostrarCitaFecha.MostrarCitaAsync(parametros);

            return Ok(new { mensaje = "La lista de las fecha se envio correctamente", Lista });
        }
    }
}