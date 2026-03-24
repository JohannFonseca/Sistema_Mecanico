using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Taller_Mecanico_de_Citas.BL;
using Sistema_de_Taller_Mecanico_de_Citas.DA;
using Sistema_de_Taller_Mecanico_de_Citas.Model;

namespace Sistema_de_Taller_Mecanico_de_Citas.SI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenesEnProcesoController : ControllerBase
    {
        private readonly IGestorDeOrdenesEnProceso gestor;

        public OrdenesEnProcesoController(IGestorDeOrdenesEnProceso Igestor)
        {
            gestor = Igestor;
        }

        // GET: api/OrdenesEnProceso
        [HttpGet("ObtengaLasOrdenesEnProceso")]
        public List<Model.OrdenDeTrabajo> ObtengaLasOrdenesEnProceso()
        {
            return gestor.ObtengaLasOrdenesEnProceso();
        }

        // GET: api/OrdenesEnProceso
        [HttpGet("ObtengaOrdenPorId/{id}")]
        public Model.OrdenDeTrabajo ObtengaOrdenPorId(int id)
        {
            return gestor.ObtengaOrdenPorId(id);
        }

        // POST: api/OrdenesEnProceso
        [HttpPost("AgregarRepuesto")]
        public IActionResult AgregarRepuesto([FromBody] RepuestoParaOrdenEnProceso repuesto)
        {
            try
            {
                gestor.AgregarRepuestoAOrden(repuesto.IdOrden, repuesto.IdRepuesto, repuesto.Cantidad);
                return Ok("Repuesto agregado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al agregar repuesto: {ex.Message}");
            }
        }


        // GET: api/OrdenesEnProceso
        [HttpGet("ObtenerRepuestosAsignadosAOrden/{id}")]
        public List<Model.OrdenRepuesto> ObtenerRepuestosAsignadosAOrden(int id)
        {
            return gestor.ObtenerRepuestosAsignadosAOrden(id);
        }

        // POST: api/OrdenesEnProceso
        [HttpPost("AgregarServicioAOrden")]
        public IActionResult AgregarServicio([FromBody] ServicioParaOrdenEnProceso servicio)
        {
            try 
            {
            gestor.AgregarServicioAOrden(servicio.IdOrden, servicio.IdServicio, servicio.Cantidad);
            return Ok("Servicio agregado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al agregar servicio: {ex.Message}");
            }

        }

        // GET: api/OrdenesEnProceso/{id}/servicios
        [HttpGet("ObtenerServiciosAsignadosAOrden/{id}")]
        public List<Model.OrdenServicio> ObtenerServiciosAsignadosAOrden(int id)
        {
            return gestor.ObtenerServiciosAsignadosAOrden(id);
        }

        // GET: api/OrdenesEnProceso/{id}/totales
        [HttpGet("ObtenerTotalesDeOrden")]
        public Model.ResumenTotal VerTotal(int id)
        {
            return gestor.ObtenerTotalesDeOrden(id);
        }
    }
}

