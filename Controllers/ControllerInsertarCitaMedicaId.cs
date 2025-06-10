using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Inserta-cita-Id")]
    public class ControllerInsertarCitaMedicaId : ControllerBase
    {
        private readonly DataInsertarCitaMedicaId _dataInsertarCitaMedicaId;

        public ControllerInsertarCitaMedicaId(DataInsertarCitaMedicaId dataInsertarCitaMedicaId)
        {
            _dataInsertarCitaMedicaId = dataInsertarCitaMedicaId;
        }

        [Authorize(Roles = "Paciente")]
        [HttpPost]
        public async Task<IActionResult> POST([FromBody] ModelInsertarCitaMedicaId parametros)
        {
            var userIdClaim = User.FindFirst("id");
            int idRegistro = int.Parse(userIdClaim.Value);

            int idPaciente = await _dataInsertarCitaMedicaId.ObtenerIdPacienteAsync(idRegistro);

            await _dataInsertarCitaMedicaId.InsertarCitaMedicaIdAsync(parametros, idPaciente);

            return Ok(new { mensaje = "Cita creada correctamente" });
        }
    }

}