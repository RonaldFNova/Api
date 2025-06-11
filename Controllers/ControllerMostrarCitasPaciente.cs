using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Mostrar-citas-pedientes-paciente")]
    public class ControllerMostrarCitasPaciente : ControllerBase
    {
        private readonly DataMostrarCitasPaciente _dataMostrarCitasPaciente;

        public ControllerMostrarCitasPaciente(DataMostrarCitasPaciente dataMostrarCitasPaciente)
        {
            _dataMostrarCitasPaciente = dataMostrarCitasPaciente;
        }


        [Authorize(Roles = "Paciente")]
        [HttpGet]
        public async Task<IActionResult> GET()
        {
            var userIdClaim = User.FindFirst("id");
            int idRegistro = int.Parse(userIdClaim.Value);

            int idPaciente = await _dataMostrarCitasPaciente.ObtenerIdPacienteAsync(idRegistro);

            var lista  = await _dataMostrarCitasPaciente.MostrarCitasPacienteAsync(idPaciente);

            return Ok(new { mensaje = "lista creada correctamente", lista });
        }
    }

}