using Microsoft.AspNetCore.Mvc;
using SistemaOrdenesReparacion.Model;
using SistemaOrdenesReparacion.DA;
using Microsoft.EntityFrameworkCore;
using SistemaOrdenesReparacion.BL;

namespace SistemaOrdenesReparacion.SI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly TallerDbContext _db;
        private readonly GestorEmail _emailService;


        public LoginController(TallerDbContext db, GestorEmail emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            var cuenta = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario.ToLower() == model.Usuario.ToLower());

            if (cuenta == null)
                return Unauthorized("Usuario o clave incorrectos.");

            if (cuenta.Rol != 1)
            {
                if (cuenta.BloqueadoHasta.HasValue && cuenta.BloqueadoHasta > DateTime.Now)
                {
                    await _emailService.AvisarIntentoAccesoBloqueadoAsync(
                        cuenta.CorreoElectronico,
                        cuenta.NombreUsuario,
                        cuenta.BloqueadoHasta.Value
                    );

                    return Unauthorized($"Cuenta bloqueada. Intente de nuevo a las {cuenta.BloqueadoHasta.Value:HH:mm:ss}.");
                }
            }

            if (cuenta.Clave != model.clave || cuenta.Rol != model.Rol || !cuenta.Activo)
            {
                if (cuenta.Rol != 1)
                {
                    cuenta.IntentosFallidos++;

                    if (cuenta.IntentosFallidos >= 2)
                    {
                        cuenta.BloqueadoHasta = DateTime.Now.AddMinutes(3);
                        cuenta.IntentosFallidos = 0;

                        await _emailService.AvisarUsuarioBloqueadoAsync(
                            cuenta.CorreoElectronico,
                            cuenta.NombreUsuario,
                            cuenta.BloqueadoHasta.Value
                        );

                        await _db.SaveChangesAsync();
                        return Unauthorized("Tu cuenta ha sido bloqueada por 3 minutos por intentos fallidos.");
                    }

                    await _db.SaveChangesAsync();
                }

                return Unauthorized("Usuario o clave incorrectos.");
            }

            cuenta.IntentosFallidos = 0;
            cuenta.BloqueadoHasta = null;
            await _db.SaveChangesAsync();

            await _emailService.EnviarCorreoInicioSesionAsync(cuenta.CorreoElectronico, cuenta.NombreUsuario, cuenta.Rol);

            var rolNombre = cuenta.Rol == 1 ? "Administrador" :
                            cuenta.Rol == 2 ? "Cliente" :
                            cuenta.Rol == 3 ? "Mecanico" : "Desconocido";

            return Ok(new
            {
                id = cuenta.Id,
                idCliente = cuenta.Id_Cliente,
                idMecanico = cuenta.Id_Mecanico,
                usuario = cuenta.NombreUsuario,
                rol = cuenta.Rol,
                nombreRol = rolNombre
            });
        }


        [HttpPost("cambiar-clave")]
        public async Task<IActionResult> CambiarClave([FromBody] CambioClave model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            if (string.IsNullOrWhiteSpace(model.NombreUsuario) ||
                string.IsNullOrWhiteSpace(model.ClaveActual) ||
                string.IsNullOrWhiteSpace(model.NuevaClave))
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            var cuenta = await _db.Usuarios.FirstOrDefaultAsync(u =>
                u.NombreUsuario.ToLower() == model.NombreUsuario.ToLower() &&
                u.Clave == model.ClaveActual);

            if (cuenta == null)
                return Unauthorized("Nombre de usuario o clave actual incorrectos.");

            cuenta.Clave = model.NuevaClave;
            cuenta.FechaUltimoCambioClave = DateTime.Now;
            await _db.SaveChangesAsync();

            await _emailService.EnviarCorreoCambioClaveAsync(cuenta.CorreoElectronico, cuenta.NombreUsuario);

            return Ok("Contraseña actualizada correctamente.");
        }


    }

}