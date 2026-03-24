using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Taller_Mecanico_de_Citas.DA;
using Sistema_de_Taller_Mecanico_de_Citas.Model;

namespace Sistema_de_Taller_Mecanico_de_Citas.SI.Controllers
{
    [ApiController]
    [Route("api/ordenes/cliente")]
    public class OrdenesClienteController : ControllerBase
    {
        private readonly TallerDbContext _context;

        public OrdenesClienteController(TallerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerOrdenesCliente([FromQuery] int idCliente, [FromQuery] string? estado = null)
        {
            if (idCliente <= 0)
                return BadRequest("Id de cliente inválido.");

            var query = _context.OrdenesDeTrabajos
                .Include(o => o.Cliente)
                .Where(o => o.Id_Cliente == idCliente)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                var estadoEnum = ObtenerEstadoDesdeTexto(estado);
                if (estadoEnum != null)
                    query = query.Where(o => o.Estado == estadoEnum);
            }


            var ordenes = await query
                .OrderByDescending(o => o.FechaDeRegistro)
                .ToListAsync();

            return Ok(ordenes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerDetalleOrden(int id, [FromQuery] int idCliente)
        {
            if (idCliente <= 0)
                return BadRequest("Id de cliente inválido.");

            var orden = await _context.OrdenesDeTrabajos
                .Include(o => o.Mecanico)
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == id && o.Id_Cliente == idCliente);

            if (orden == null)
                return NotFound("Orden no encontrada para ese cliente.");

            var repuestos = await _context.OrdenesDeTrabajoInventarioDeRepuestos
                .Include(r => r.Repuesto)
                .Where(r => r.Id_OrdenesDeTrabajos == id)
                .ToListAsync();

            var servicios = await _context.OrdenesDeTrabajoInventarioDeServicios
                .Include(s => s.Servicio)
                .Where(s => s.Id_OrdenesDeTrabajo == id)
                .ToListAsync();

            var resultado = new
            {
                Orden = orden,
                Repuestos = repuestos,
                Servicios = servicios,
                TotalRepuestos = repuestos.Sum(r => r.Total),
                TotalServicios = servicios.Sum(s => s.Total),
                TotalOrden = repuestos.Sum(r => r.Total) + servicios.Sum(s => s.Total)
            };

            return Ok(resultado);
        }
        private EstadoOrden? ObtenerEstadoDesdeTexto(string estadoTexto)
        {
            foreach (var valor in Enum.GetValues(typeof(EstadoOrden)))
            {
                var campo = valor.GetType().GetField(valor.ToString());
                var display = campo?.GetCustomAttributes(typeof(DisplayAttribute), false)
                                    .FirstOrDefault() as DisplayAttribute;

                if (display != null && display.Name == estadoTexto)
                    return (EstadoOrden)valor;
            }
            return null;
        }

    }

}
