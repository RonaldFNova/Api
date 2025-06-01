using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("/Api/Insertar-cita")]

    public class ControllerInsertarCitaMedica : ControllerBase
    {

        private readonly DataInsertarCitaMedica _dataInsertarCitaMedica;
        public ControllerInsertarCitaMedica(DataInsertarCitaMedica dataInsertarCitaMedica)
        {
            _dataInsertarCitaMedica = dataInsertarCitaMedica;
        }

        [HttpPost]
        [Authorize(Roles = "Paciente,Administrador")]
        public async Task<IActionResult> POST([FromBody] ModelInsertarCitaMedica parametros)
        {
            var userIdClaim = User.FindFirst("id");
            int idRegistro = int.Parse(userIdClaim.Value);

            int idPaciente = await _dataInsertarCitaMedica.ObtenerIdPacienteAsync(idRegistro);

            int idMedico = await _dataInsertarCitaMedica.InsertarNombreIdAsync(parametros);

            await _dataInsertarCitaMedica.InsertarCitaMedicaAsync(parametros, idMedico, idPaciente);

            return Ok(new { Mensaje = "La cita se a agendado correctamente" });

        }
    }
}