
using API.Data;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Mostrar-Doctores")]
    public class ControllerMostrarDoctores : ControllerBase
    {
        private readonly DataMostrarDoctores _dataMostrarDoctores;
        public ControllerMostrarDoctores(DataMostrarDoctores dataMostrarDoctores)
        {
            _dataMostrarDoctores = dataMostrarDoctores;
        }

        [HttpGet]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult<List<ModelMostrarDoctores>>> GET()
        {
            var lista = await _dataMostrarDoctores.MostrarDoctoresAsync();
            return Ok(lista);
        }
    }

}