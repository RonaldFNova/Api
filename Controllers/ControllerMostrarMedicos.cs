
using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Mostrar-Medicos")]
    public class ControllerMostrarMedicos : ControllerBase
    {
        private readonly DataMostrarMedicos _dataMostrarMedicos;
        public ControllerMostrarMedicos(DataMostrarMedicos dataMostrarMedicos)
        {
            _dataMostrarMedicos = dataMostrarMedicos;
        }

        [HttpGet]
        [Authorize(Roles = "Paciente,Administrador")]
        public async Task<ActionResult<List<ModelMostrarMedicos>>> GET()
        {
            var lista = await _dataMostrarMedicos.MostrarMedicosAsync();
            return Ok(lista);
        }
    }

}