using Microsoft.AspNetCore.Mvc;
using SistemaOrdenesReparacion.DA;
using Microsoft.EntityFrameworkCore;

namespace SistemaOrdenesReparacion.SI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersDebugController : ControllerBase
    {
        private readonly TallerDbContext _db;
        public UsersDebugController(TallerDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _db.Usuarios.Select(u => new { u.NombreUsuario, u.Clave, u.Rol }).ToListAsync();
            return Ok(users);
        }
    }
}
