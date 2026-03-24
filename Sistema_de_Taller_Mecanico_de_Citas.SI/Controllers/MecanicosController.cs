using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Taller_Mecanico_de_Citas.DA;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System.Net;
using System.Net.Mail;

namespace Sistema_de_Taller_Mecanico_de_Citas.SI.Controllers
{
    [ApiController]
    [Route("api/mecanicos")]
    public class MecanicosController : ControllerBase
    {
        private readonly TallerDbContext _context;

        public MecanicosController(TallerDbContext context)
        {
            _context = context;
        }

        // GET: api/mecanicos
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var mecanicos = await _context.Mecanicos
                .Select(m => new
                {
                    m.Id,
                    m.Nombre,
                    m.Apellidos,
                    m.Identificacion,
                    m.CorreoElectronico
                }).ToListAsync();

            return Ok(mecanicos);
        }

        // GET: api/mecanicos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var mecanico = await _context.Mecanicos.FirstOrDefaultAsync(m => m.Id == id);
            if (mecanico == null) return NotFound("Mecánico no encontrado.");

            return Ok(mecanico);
        }

        // POST: api/mecanicos
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Mecanico mecanico)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Guardar mecánico primero
            _context.Mecanicos.Add(mecanico);
            await _context.SaveChangesAsync();

            // Generar credenciales
            string claveGenerada = GenerarClave();
            string nombreUsuario = GenerarNombreUsuario(mecanico);

            var usuario = new Usuario
            {
                NombreUsuario = nombreUsuario,
                Clave = claveGenerada,
                CorreoElectronico = mecanico.CorreoElectronico,
                Rol = 3, // Rol Mecanico
                Id_Mecanico = mecanico.Id
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Enviar correo
            EnviarCorreoBienvenida(mecanico.CorreoElectronico, mecanico.Nombre, usuario.NombreUsuario, claveGenerada);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = mecanico.Id }, mecanico);
        }

        // PUT: api/mecanicos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] Mecanico mecanico)
        {
            if (id != mecanico.Id)
                return BadRequest("Id de mecánico no coincide.");

            var existente = await _context.Mecanicos.FindAsync(id);
            if (existente == null)
                return NotFound("Mecánico no encontrado.");

            existente.Nombre = mecanico.Nombre;
            existente.Apellidos = mecanico.Apellidos;
            existente.Identificacion = mecanico.Identificacion;
            existente.CorreoElectronico = mecanico.CorreoElectronico;

            await _context.SaveChangesAsync();

            return NoContent();
        }

    

        // Métodos privados auxiliares

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
    }
}
