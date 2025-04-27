using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Security
{
    public class EmailService
    {
        private readonly string ApiKey;

        public EmailService(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            ApiKey = builder.GetSection("SendGrid:ApiKey").Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new SendGridClient(ApiKey);
            var from = new EmailAddress("rdevia1@udi.edu.co", "MediConnet");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
