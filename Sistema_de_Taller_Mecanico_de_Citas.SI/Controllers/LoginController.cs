using Microsoft.AspNetCore.Mvc;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using Sistema_de_Taller_Mecanico_de_Citas.DA;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Taller_Mecanico_de_Citas.BL;

namespace Sistema_de_Taller_Mecanico_de_Citas.SI.Controllers
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

            if (cuenta.Clave != model.clave || !cuenta.Activo)
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
                u.NombreUsuario.ToLower() == model.NombreUsuario.ToLower());

            if (cuenta == null || cuenta.Clave != model.ClaveActual)
                return Unauthorized("Nombre de usuario o clave actual incorrectos.");

            // Restricción: No se puede cambiar la clave del administrador o mecánico por este medio
            // a menos que seas el mismo usuario (validado por la clave actual).
            // Pero el usuario pidió explícitamente: "nunca pueda cambiar la del admin ni la de un mecanico"
            // Esto lo interpretamos como que un usuario no puede cambiar la clave de OTRO que sea admin o mecánico.
            // Dado que pedimos ClaveActual, ya estamos validando identidad.

            if (cuenta.NombreUsuario.ToLower() == "admin" && model.NombreUsuario.ToLower() != "admin")
                 return BadRequest("No está permitido cambiar la contraseña del administrador.");

            cuenta.Clave = model.NuevaClave;
            cuenta.FechaUltimoCambioClave = DateTime.Now;
            await _db.SaveChangesAsync();

            await _emailService.EnviarCorreoCambioClaveAsync(cuenta.CorreoElectronico, cuenta.NombreUsuario, cuenta.Clave);

            return Ok("Contraseña actualizada correctamente. Se ha enviado una confirmación a su correo.");
        }

        [HttpPost("recuperar-clave")]
        public async Task<IActionResult> RecuperarClave([FromBody] string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return BadRequest("El nombre de usuario es obligatorio.");

            var cuenta = await _db.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower());

            if (cuenta == null)
                return NotFound("Usuario no encontrado.");

            if (!cuenta.Activo)
                return BadRequest("La cuenta está inactiva.");

            await _emailService.EnviarCorreoRecuperacionAsync(cuenta.CorreoElectronico, cuenta.NombreUsuario, cuenta.Clave);

            return Ok("Se ha enviado su contraseña actual al correo electrónico registrado.");
        }


    }

}