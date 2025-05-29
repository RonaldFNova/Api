using System.Security.Claims;
using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Horario-medico")]
    public class ControllerHorarioMedico : ControllerBase
    {
        private readonly DataHorarioMedico _dataHorarioMedico;
        public ControllerHorarioMedico(DataHorarioMedico dataHorarioMedico)
        {
            _dataHorarioMedico = dataHorarioMedico;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> POST([FromBody] ModelHorarioMedico parametros)
        {

            var userIdClaim = User.FindFirst("id");

            int id = int.Parse(userIdClaim.Value);

            id = await _dataHorarioMedico.ObtenerIdProfesionalAsync(id);

            if (parametros.Dia == null || parametros.Final == null || parametros.Inicio == null)
                await _dataHorarioMedico.InsertHorarioPorDefectoAsync(id);

            else
                await _dataHorarioMedico.InsertHorarioMedicoAsync(id, parametros);
        

            return Ok(new { mensaje = "Horarios insertados correctamente" });
        }
    }

}