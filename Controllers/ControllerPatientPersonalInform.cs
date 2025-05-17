using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Informacion-Personal-Paciente")]
    public class ControllerPatientPersonalInform : ControllerBase
    {
        private readonly DataPatientPersonalInform _dataPatientPersonalInform;

        public ControllerPatientPersonalInform(DataPatientPersonalInform dataPatientPersonalInform)
        {
            _dataPatientPersonalInform = dataPatientPersonalInform;
        }

        [HttpPost]
        [Authorize(Roles = "Paciente")]

        public async Task<IActionResult> InsertarInformacionPaciente([FromBody] ModelPatientPersonalInform personalInform)
        {
            await _dataPatientPersonalInform.InsertInformacionPersonal(personalInform);

            return Ok(new { mensaje = "La informacion enviada al paciente se registro correctamente"});           
        }
    
    }
}
