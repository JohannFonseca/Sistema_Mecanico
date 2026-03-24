using Microsoft.Extensions.Configuration;
using Sistema_de_Taller_Mecanico_de_Citas.DA;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;

namespace Sistema_de_Taller_Mecanico_de_Citas.BL
{
    public class GestorMecanicos : IGestorMecanicos
    {
        private readonly TallerDbContext _dbContext;

        public GestorMecanicos(TallerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Mecanico> ObtenerTodos()
        {
            return _dbContext.Mecanicos
                .Select(m => new Mecanico
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Apellidos = m.Apellidos,
                    Identificacion = m.Identificacion,
                    CorreoElectronico = m.CorreoElectronico 

                }).ToList();
        }

        public Mecanico ObtenerPorId(int id)
        {
            return _dbContext.Mecanicos.FirstOrDefault(m => m.Id == id);
        }

        public void Agregar(Mecanico mecanico)
        {
            _dbContext.Mecanicos.Add(mecanico);
            _dbContext.SaveChanges(); 

            string claveGenerada = GenerarClave();

            var usuario = new Usuario
            {
                NombreUsuario = GenerarNombreUsuario(mecanico),
                Clave = claveGenerada,
                CorreoElectronico = mecanico.CorreoElectronico,
                Rol = 3,
                Id_Mecanico = mecanico.Id
            };

            _dbContext.Usuarios.Add(usuario);
            _dbContext.SaveChanges();

            EnviarCorreoBienvenida(mecanico.CorreoElectronico, mecanico.Nombre, usuario.NombreUsuario, claveGenerada);
        }


        private string GenerarNombreUsuario(Mecanico mecanico)
        {
            return (mecanico.Nombre + "." + mecanico.Apellidos).ToLower().Replace(" ", "") + DateTime.Now.Ticks.ToString()[^4..];
        }

        private string GenerarClave()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void EnviarCorreoBienvenida(string correo, string nombre, string usuario, string clave)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress("tutallerdeconfianza2025@gmail.com");
                mail.To.Add(correo);
                mail.Subject = "Bienvenido al Sistema de Órdenes de Reparación";
                mail.Body = $"Hola {nombre},\n\nTu cuenta ha sido creada exitosamente.\n\nUsuario: {usuario}\nContraseña: {clave}\n\nTe recomendamos cambiar tu contraseña después de ingresar.\n\nGracias por registrarte.";
                mail.IsBodyHtml = false;

                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("tutallerdeconfianza2025@gmail.com", "gkcp prdt mffy srdf"),
                    EnableSsl = true
                };

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
        }



        public void Editar(Mecanico mecanico)
        {
            var existente = _dbContext.Mecanicos.FirstOrDefault(m => m.Id == mecanico.Id);
            if (existente != null)
            {
                existente.Nombre = mecanico.Nombre;
                existente.Apellidos = mecanico.Apellidos;
                existente.Identificacion = mecanico.Identificacion;
                existente.CorreoElectronico = mecanico.CorreoElectronico;
                _dbContext.SaveChanges();
            }
        }
       public void Eliminar(int id)
{
    var mecanico = _dbContext.Mecanicos.FirstOrDefault(m => m.Id == id);
    if (mecanico != null)
    {
        // Eliminar usuario asociado
        var usuario = _dbContext.Usuarios.FirstOrDefault(u => u.Id_Mecanico == mecanico.Id);
        if (usuario != null)
        {
            _dbContext.Usuarios.Remove(usuario);
        }

        // Eliminar todas las órdenes asignadas al mecánico
        var ordenes = _dbContext.OrdenesDeTrabajos.Where(o => o.Id_Mecanico == mecanico.Id).ToList();
        if (ordenes.Any())
        {
            _dbContext.OrdenesDeTrabajos.RemoveRange(ordenes);
        }

        // Finalmente eliminar el mecánico
        _dbContext.Mecanicos.Remove(mecanico);

        _dbContext.SaveChanges();
    }
}

        }

    }


