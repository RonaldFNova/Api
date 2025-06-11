using API.Data;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("/Api/Verificacion-sms")]
    public class ControllerEnviarCodigoSms : ControllerBase
    {
        private readonly DataEnviarCodigoSms _dataEnviarCodigoSms;
        private readonly CodigoVerificacionService _codigoVerificacionService;
        private readonly JwtService _jwtService;
        private readonly SmsCodeVerificacionServices _smsCodeVerificacionServices;
        public ControllerEnviarCodigoSms(CodigoVerificacionService codigoVerificacionService, SmsCodeVerificacionServices smsCodeVerificacionServices, DataEnviarCodigoSms dataEnviarCodigoSms, JwtService jwtService)
        {
            _codigoVerificacionService = codigoVerificacionService;
            _smsCodeVerificacionServices = smsCodeVerificacionServices;
            _dataEnviarCodigoSms = dataEnviarCodigoSms;
            _jwtService = jwtService;
        }


        [HttpPost("Enviar-codigo")]
        [Authorize(Roles = "Administrador,Paciente,Enfermero")]
        public async Task<IActionResult> POST()
        {
            var userIdClaim = User.FindFirst("id");

            int userId = int.Parse(userIdClaim.Value);

            string cell = await _dataEnviarCodigoSms.ObtenerCellAsync(userId);

            string codigoSms = _codigoVerificacionService.GenerarCodigo();

            var tokenCodigo =  _jwtService.GenerateVerificationToken(userIdClaim.Value, codigoSms);

            await _smsCodeVerificacionServices.EnviarSms(cell, codigoSms);

            return Ok(new { Mensaje = "El codigo de verificacion de cell se ha enviado exitosamente",tokenCodigo });

        }


    }
}