using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Sistema_de_Taller_Mecanico_de_Citas.BL
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
        public async Task EnviarCorreoCambioClaveAsync(string destino, string usuario, string nuevaClave)
        {
            var asunto = $"Confirmación de cambio de clave - {usuario}";
            var cuerpo = $"Hola {usuario},\n\nLe informamos que la clave de su cuenta ha sido cambiada exitosamente.\n\nSu nueva clave es: {nuevaClave}\n\nSi no realizó este cambio, por favor contacte al administrador de inmediato.";
            await Enviar(destino, asunto, cuerpo);
        }

        public async Task EnviarCorreoRecuperacionAsync(string destino, string usuario, string clave)
        {
            var asunto = $"Recuperación de contraseña - {usuario}";
            var cuerpo = $"Hola {usuario},\n\nHa solicitado la recuperación de su contraseña.\n\nSu contraseña actual es: {clave}\n\nLe recomendamos cambiarla una vez que ingrese al sistema.";
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

        public async Task EnviarCorreoFacturaAsync(string destino, string cliente, string orderId, string detalleHtml, decimal total, string metodoPago)
        {
            var asunto = $"Factura de Servicios - Orden #{orderId} - Sistema de Taller";
            var cuerpo = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #7a4f01; text-align: center;'>Factura de Servicios</h2>
                    <p>Hola <strong>{cliente}</strong>,</p>
                    <p>Gracias por confiar en nuestro taller. Adjuntamos el detalle de su servicio:</p>
                    <hr/>
                    <table style='width: 100%; border-collapse: collapse;'>
                        <thead>
                            <tr style='background-color: #f8f9fa;'>
                                <th style='padding: 10px; text-align: left; border-bottom: 2px solid #dee2e6;'>Detalle</th>
                                <th style='padding: 10px; text-align: right; border-bottom: 2px solid #dee2e6;'>Precio</th>
                            </tr>
                        </thead>
                        <tbody>
                            {detalleHtml}
                        </tbody>
                        <tfoot>
                            <tr>
                                <td style='padding: 10px; text-align: right; font-weight: bold;'>Total:</td>
                                <td style='padding: 10px; text-align: right; font-weight: bold; color: #7a4f01;'>₡{total:N0}</td>
                            </tr>
                        </tfoot>
                    </table>
                    <p style='margin-top: 20px;'><strong>Método de Pago:</strong> {metodoPago}</p>
                    <p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                    <hr/>
                    <p style='font-size: 12px; color: #666; text-align: center;'>Este es un comprobante electrónico generado por el Sistema de Taller Mecánico.</p>
                </div>";
            await Enviar(destino, asunto, cuerpo, true);
        }

        private async Task Enviar(string destino, string asunto, string cuerpo, bool isHtml = false)
        {
            var remitente = _config["EmailSettings:Remitente"];
            var alias = _config["EmailSettings:Alias"];

            using var mensaje = new MailMessage(remitente, destino, asunto, cuerpo);
            mensaje.IsBodyHtml = isHtml;
            mensaje.From = new MailAddress(remitente, alias);

            using var smtp = ConfigurarSmtp();
            await smtp.SendMailAsync(mensaje);
        }
    }
}
