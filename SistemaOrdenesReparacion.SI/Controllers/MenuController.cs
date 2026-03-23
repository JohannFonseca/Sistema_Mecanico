using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaOrdenesReparacion.SI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetMenuInfo()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var nombreUsuario = User.Identity.Name;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                NombreUsuario = nombreUsuario,
                Rol = rol
            });
        }
    }
}

