using DotNetEnv;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace API.Security
{
    public class SmsCodeVerificacionServices
    {
        private readonly string _AuthToken;

        private readonly string _apiAccountSID;

        private readonly string _PhoneNumber;
        public SmsCodeVerificacionServices()
        {
            string apiAccountSID = "/etc/secrets/AccountSID";

            if (File.Exists(apiAccountSID)) _apiAccountSID = File.ReadAllText(apiAccountSID).Trim();

            else
            {
                Env.Load();
                _apiAccountSID = Environment.GetEnvironmentVariable("AccountSID") ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(_apiAccountSID)) throw new Exception("No se ha configurado AccountSID.");


            string apiAuthToken = "/etc/secrets/AuthToken";

            if (File.Exists(apiAuthToken)) _AuthToken = File.ReadAllText(apiAuthToken).Trim();

            else
            {
                Env.Load();
                _AuthToken = Environment.GetEnvironmentVariable("AuthToken") ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(_AuthToken)) throw new Exception("No se ha configurado AuthToken.");


            string apiPhoneNumber = "/etc/secrets/PhoneNumber";

            if (File.Exists(apiPhoneNumber)) _PhoneNumber = File.ReadAllText(apiPhoneNumber).Trim();

            else
            {
                Env.Load();
                _PhoneNumber = Environment.GetEnvironmentVariable("PhoneNumber") ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(_PhoneNumber)) throw new Exception("No se ha configurado PhoneNumber.");


        }

        public async Task EnviarSms(string cell, string CodigoVerificacion)
        {
            await Task.Run(() =>
            {
                TwilioClient.Init(_apiAccountSID, _AuthToken);

                var mensaje = MessageResource.Create(
                    to: new PhoneNumber(cell),
                    from: new PhoneNumber(_PhoneNumber),
                    body: $"Su código de verificación para (Mediconnet) es: {CodigoVerificacion}"
                );
            });
        }

    }
}