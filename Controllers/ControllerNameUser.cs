using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("/Api/Nombre-Usuario")]
    public class ControllerNameUser : ControllerBase
    {
        private readonly DataNameUser _dataNameUser;

        public ControllerNameUser(DataNameUser dataNameUser)
        {
            _dataNameUser = dataNameUser;
        }

        [HttpPost]
        [Authorize(Roles = "Paciente,Medico,Administrador")]
        public async Task<IActionResult> POST()
        {
            var userIdClaim = User.FindFirst("id");
            int id = int.Parse(userIdClaim.Value);

            var NombreCompleto = await _dataNameUser.ObtenerNameUserAsync(id);

            return Ok(new { mensaje = "El nombre se envio correctamente", NombreCompleto });
        }
    }
}