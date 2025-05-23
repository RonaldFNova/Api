using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("Api/Informacion-Personal-doctor")]
    public class ControllerDoctorPersonalInform : ControllerBase
    {
        public readonly DataDoctorPersonalInform _dataDoctorPersonalInform;
        public ControllerDoctorPersonalInform(DataDoctorPersonalInform dataDoctorPersonalInform)
        {
            _dataDoctorPersonalInform = dataDoctorPersonalInform;
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> POST([FromBody] ModelDoctorPersonalInform parametros)
        {
            await _dataDoctorPersonalInform.InsertarDoctorAsync(parametros);
            return Ok (new { mensaje = "La informacion enviada del doctor se registro correctamente"});
        }

    }

}