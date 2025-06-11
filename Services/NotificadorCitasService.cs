using API.Data;

namespace API.Services
{
    public class NotificadorCitasService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificadorCitasService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var data = scope.ServiceProvider.GetRequiredService<DataNotificarCitas>();
                var email = scope.ServiceProvider.GetRequiredService<EmailService>();

                var citas = await data.ObtenerCitasDeMananaAsync();

                foreach (var cita in citas)
                {
                    var fecha = cita.Fecha.ToString("dd/MM/yyyy");
                    var hora = cita.HoraInicio.ToString(@"hh\:mm");

                    var htmlContent = $@"
                    <!DOCTYPE html>
                    <html lang='es'>
                    <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{
                        font-family: Arial, sans-serif;
                        background-color: #f5f5f5;
                        margin: 0;
                        padding: 0;
                        }}
                        .container {{
                        background-color: #ffffff;
                        max-width: 600px;
                        margin: 30px auto;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                        text-align: center;
                        padding-bottom: 20px;
                        }}
                        .brand {{
                        font-size: 32px;
                        font-weight: bold;
                        color: #26a541;
                        margin-bottom: 10px;
                        }}
                        .title {{
                        font-size: 24px;
                        margin-bottom: 10px;
                        color: #333333;
                        }}
                        .message {{
                        font-size: 16px;
                        color: #555555;
                        margin-bottom: 20px;
                        }}
                        .info-box {{
                        background-color: #f0f0f0;
                        padding: 15px;
                        border-radius: 5px;
                        font-size: 18px;
                        color: #111;
                        margin-bottom: 20px;
                        }}
                        .footer {{
                        text-align: center;
                        font-size: 12px;
                        color: #999999;
                        margin-top: 20px;
                        }}
                        a {{
                        color: #1a73e8;
                        text-decoration: none;
                        }}
                    </style>
                    </head>
                    <body>
                    <div class='container'>
                        <div class='header'>
                        <div class='brand'>MediConnet</div>
                        <div class='title'>Recordatorio de Cita Médica</div>
                        </div>
                        <div class='message'>
                        Hola {cita.NombrePaciente},<br><br>
                        Este es un recordatorio de que tienes una cita médica agendada para mañana:
                        </div>
                        <div class='info-box'>
                        📅 <strong>Fecha:</strong> {fecha}<br>
                        🕒 <strong>Hora:</strong> {hora}
                        </div>
                        <div class='message'>
                        Por favor asegúrate de estar en casa 10 minutos antes. Si tienes preguntas, comunícate con nosotros.
                        </div>
                        <div class='footer'>
                        © 2025 MediConnet. Todos los derechos reservados.<br>
                        <a href='#'>Ayuda</a> • <a href='#'>Política de Privacidad</a> • <a href='#'>Términos del servicio</a>
                        </div>
                    </div>
                    </body>
                    </html>";

                    await email.SendEmail24HAsync(cita.Correo, "Recordatorio de Cita Médica", htmlContent);
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}