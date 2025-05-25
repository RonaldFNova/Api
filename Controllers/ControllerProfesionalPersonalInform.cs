using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("Api/Informacion-Personal-Profesional")]
    public class ControllerProfesionalPersonalInform : ControllerBase
    {
        public readonly DataProfesionalPersonalInform _dataProfesionalPersonalInform;
        public ControllerProfesionalPersonalInform(DataProfesionalPersonalInform dataProfesionalPersonalInform)
        {
            _dataProfesionalPersonalInform = dataProfesionalPersonalInform;
        }

        [HttpPost]
        [Authorize(Roles = "Medico,Administrador,Enfermero")]
        public async Task<IActionResult> POST([FromBody] ModelProfesionalPersonalInform parametros)
        {
            await _dataProfesionalPersonalInform.InsertarProfesionalAsync(parametros);
            return Ok (new { mensaje = "La informacion enviada del doctor se registro correctamente"});
        }

    }

}