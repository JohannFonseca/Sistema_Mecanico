using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SistemaOrdenesReparacion.BL
{
    public class GestorEmail
    {
        private readonly IConfiguration _config;

        public GestorEmail(IConfiguration config)
        {
            _config = config;
        }

        private SmtpClient ConfigurarSmtp()
        {
            return new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Remitente"],
                    _config["EmailSettings:Password"]
                ),
                EnableSsl = true
            };
        }

        public async Task EnviarCorreoInicioSesionAsync(string destino, string usuario, int rol)
        {
            var asunto = $"Inicio de sesión del usuario {usuario}";
            var cuerpo = $"Usted inició sesión el día {DateTime.Now:dd/MM/yyyy} a las {DateTime.Now:HH:mm}.";
            await Enviar(destino, asunto, cuerpo);
        }
        public async Task EnviarCorreoCambioClaveAsync(string destino, string usuario)
        {
            var asunto = $"Cambio de clave del usuario {usuario}";
            var cuerpo = $"Le informamos que la cuenta del usuario {usuario} ha cambiado su clave.";
            await Enviar(destino, asunto, cuerpo);
        }


        public async Task AvisarUsuarioBloqueadoAsync(string destino, string usuario, DateTime finBloqueo)
        {
            var asunto = "Usuario Bloqueado";
            var cuerpo = $"Le informamos que la cuenta del usuario {usuario} se encuentra bloqueada por 3 minutos. " +
                         $"Por favor intente ingresar el día {finBloqueo:dd/MM/yyyy} a las {finBloqueo:HH:mm}.";
            await Enviar(destino, asunto, cuerpo);
        }

        public async Task AvisarIntentoAccesoBloqueadoAsync(string destino, string usuario, DateTime finBloqueo)
        {
            var asunto = $"Intento de inicio de sesión del usuario {usuario} bloqueado";
            var cuerpo = $"Le informamos que la cuenta del usuario {usuario} se encuentra bloqueada por 3 minutos. " +
                         $"Por favor intente ingresar el día {finBloqueo:dd/MM/yyyy} a las {finBloqueo:HH:mm}.";
            await Enviar(destino, asunto, cuerpo);
        }

        private async Task Enviar(string destino, string asunto, string cuerpo)
        {
            var remitente = _config["EmailSettings:Remitente"];
            var alias = _config["EmailSettings:Alias"];

            using var mensaje = new MailMessage(remitente, destino, asunto, cuerpo);
            mensaje.IsBodyHtml = false;
            mensaje.From = new MailAddress(remitente, alias);

            using var smtp = ConfigurarSmtp();
            await smtp.SendMailAsync(mensaje);
        }
    }
}
