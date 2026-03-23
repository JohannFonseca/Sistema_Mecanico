using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaOrdenesReparacion.DA;
using SistemaOrdenesReparacion.Model;

namespace SistemaOrdenesReparacion.SI.Controllers

{

    [ApiController]
    [Route("api/ordenes/canceladas")]
    public class OrdenesCanceladasController : ControllerBase
    {
        private readonly TallerDbContext _context;

        public OrdenesCanceladasController(TallerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetOrdenesCanceladas([FromQuery] string? filtroCliente = null)
        {
            var query = _context.OrdenesDeTrabajos
                .Include(o => o.Cliente)
                .Where(o => o.Estado == EstadoOrden.Cancelado);

            if (!string.IsNullOrEmpty(filtroCliente))
                query = query.Where(o => o.Cliente.Nombre.Contains(filtroCliente));

            var lista = query.ToList();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public IActionResult GetDetalleOrdenCancelada(int id)
        {
            var orden = _context.OrdenesDeTrabajos
                .Include(o => o.Cliente)
                .FirstOrDefault(o => o.Id == id && o.Estado == EstadoOrden.Cancelado);

            if (orden == null)
                return NotFound();

            return Ok(orden);
        }
    }
}
