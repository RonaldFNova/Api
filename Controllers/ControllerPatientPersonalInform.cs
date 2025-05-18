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

        public async Task<IActionResult> POST([FromBody] ModelPatientPersonalInform parametros)
        {
            await _dataPatientPersonalInform.InsertInformacionPersonal(parametros);

            return Ok(new { mensaje = "La informacion enviada al paciente se registro correctamente" });
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<List<ModelPatientPersonalInform>>> GET()
        {
            var lista = await _dataPatientPersonalInform.MostrarInformacionPersonal();
            return Ok(new { mensaje = "Pacientes pedidos correctamente",lista});;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]

        public async Task<IActionResult> PUT(int id,[FromBody] ModelPatientPersonalInform parametros)
        {
            parametros.Id = id;

            await _dataPatientPersonalInform.EditarInformacionPersonal(parametros);

            return Ok(new { mensaje = "La actualizacion enviada al paciente se registro correctamente" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]

        public async Task<IActionResult> DELETE(int id)
        {
            await _dataPatientPersonalInform.EliminarInformacionPersonal(id);

            return Ok(new { mensaje = "La eliminacion al paciente se borro correctamente"});           
        }    
    }
}
