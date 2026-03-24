using Sistema_de_Taller_Mecanico_de_Citas.Model;
using Sistema_de_Taller_Mecanico_de_Citas.DA;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace Sistema_de_Taller_Mecanico_de_Citas.BL
{
    public class GestorClientes : IGestorClientes
    {
        private readonly TallerDbContext _context;

        public GestorClientes(TallerDbContext context)
        {
            _context = context;
        }

        public void AgregueCliente(Cliente cliente)
        {
            // 1. Guardar cliente
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            // 2. Generar contraseña aleatoria
            string claveGenerada = GenerarClave();

            // 3. Crear usuario asociado
            var usuario = new Usuario
            {
                NombreUsuario = GenerarNombreUsuario(cliente),
                Clave = claveGenerada,
                CorreoElectronico = cliente.CorreoElectronico,
                Rol = 2, // rol de cliente
                Id_Cliente = cliente.Id
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            // 4. Enviar correo con clave
            EnviarCorreoBienvenida(cliente.CorreoElectronico, cliente.Nombre, usuario.NombreUsuario, claveGenerada);
        }

        private string GenerarNombreUsuario(Cliente cliente)
        {
            return (cliente.Nombre + "." + cliente.Apellidos).ToLower().Replace(" ", "") + DateTime.Now.Ticks.ToString()[^4..];
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

        public void EditeCliente(Cliente cliente)
        {
            var clienteEnDb = _context.Clientes.Find(cliente.Id);
            if (clienteEnDb != null)
            {
                clienteEnDb.Identificacion = cliente.Identificacion;
                clienteEnDb.Nombre = cliente.Nombre;
                clienteEnDb.Apellidos = cliente.Apellidos;
                clienteEnDb.CorreoElectronico = cliente.CorreoElectronico; 

                _context.SaveChanges();
            }
        }




        public Cliente? ObtengaClientePorId(int id)
        {
            return _context.Clientes.FirstOrDefault(c => c.Id == id);
        }

        public List<Cliente> MuestreLaListaDeClientes(string? filtro)
        {
            var query = _context.Clientes.AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(c => c.Nombre.Contains(filtro));
            }
            return query.ToList();
        }

    }
}
