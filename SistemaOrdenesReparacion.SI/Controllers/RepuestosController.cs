using Microsoft.AspNetCore.Mvc;
using SistemaOrdenesReparacion.BL;
using SistemaOrdenesReparacion.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SistemaOrdenesReparacion.SI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepuestosController : ControllerBase
    {
        private readonly IGestorDeRepuestos _gestor;

        public RepuestosController(IGestorDeRepuestos gestor)
        {
            _gestor = gestor;
        }

        // GET: api/<RepuestosController>
        [HttpGet("ObtenerRepuestos")]
        public List<Model.InventarioDeRepuesto> ObtenerRepuestos()
        {
            return _gestor.ObtenerRepuestos();
        }

        // GET api/<RepuestosController>/5
        [HttpGet("ObtenerRepuestoPorId/{id}")]
        public Model.InventarioDeRepuesto ObtenerRepuestoPorId(int id)
        {
            return _gestor.ObtenerRepuestoPorId(id);
        }

        [HttpPost("AgregarRepuesto")]
        public void AgregarRepuesto([FromBody] InventarioDeRepuesto repuesto)
        {
            _gestor.AgregarRepuesto(repuesto);
        }

        [HttpPut("EditeUnRepuesto/{id}")]
        public void EditeUnRepuesto(int id, [FromBody] Model.InventarioDeRepuesto repuesto)
        {
            _gestor.EditeUnRepuesto(repuesto.Nombre, repuesto.Descripcion, repuesto.Precio, id);
        }
    }
}
