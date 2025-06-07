using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("Api/Lista-Especialidades")]
    public class ControllerListaEspecialidades : ControllerBase
    {
        private readonly DataListaEspecialidades _dataListaEspecialidades;

        public ControllerListaEspecialidades(DataListaEspecialidades dataListaEspecialidades)
        {
            _dataListaEspecialidades = dataListaEspecialidades;
        }

        [Authorize(Roles = "Paciente")]
        [HttpGet]

        public async Task<IActionResult> GET()
        {

            var ListaEspecialidades = await _dataListaEspecialidades.ObtenerListaEspecialidadesAsync();

            return Ok(new { mensaje = "Lista de Especialidades enviada correctamente", ListaEspecialidades }); 
        }
    }
}