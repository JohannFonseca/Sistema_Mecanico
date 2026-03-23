using Microsoft.AspNetCore.Mvc;
using SistemaOrdenesReparacion.BL;
using SistemaOrdenesReparacion.Model;
using System.Collections.Generic;
using System.Linq;

namespace SistemaOrdenesReparacion.SI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenesDeTrabajoController : ControllerBase
    {
        private readonly IGestorOrdenesDeTrabajo _gestorOrdenes;
        private readonly IGestorMecanicos _gestorMecanicos;

        public OrdenesDeTrabajoController(IGestorOrdenesDeTrabajo gestorOrdenes, IGestorMecanicos gestorMecanicos)
        {
            _gestorOrdenes = gestorOrdenes;
            _gestorMecanicos = gestorMecanicos;
        }

        [HttpGet("MuestreLaListaDeOrdenes")]
        public List<OrdenDeTrabajo> MuestreLaListaDeOrdenes([FromQuery] string filtroCliente = null)
        {
            return _gestorOrdenes.MuestreLaListaDeOrdenes(filtroCliente);
        }

        [HttpPost("AgregueOrdenDeTrabajo")]
        public IActionResult AgregueOrdenDeTrabajo([FromBody] OrdenDeTrabajo orden)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                orden.FechaDeRegistro = System.DateTime.Now;
                orden.Estado = EstadoOrden.Registrado;

                _gestorOrdenes.AgregueOrdenDeTrabajo(orden);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("EditeOrdenDeTrabajo/{id}")]
        public IActionResult EditeOrdenDeTrabajo(int id, [FromBody] OrdenDeTrabajo orden)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != orden.Id)
                return BadRequest("El ID de la orden no coincide con el parámetro.");

            try
            {
                _gestorOrdenes.EditeOrdenDeTrabajo(orden);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("CanceleOrdenDeTrabajo/{id}")]
        public IActionResult CanceleOrdenDeTrabajo(int id, [FromQuery] string motivoCancelacion)
        {
            if (string.IsNullOrWhiteSpace(motivoCancelacion))
                return BadRequest("Debe indicar el motivo de cancelación.");

            try
            {
                _gestorOrdenes.CanceleOrdenDeTrabajo(id, motivoCancelacion);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("IniciarOrden/{id}/{mecanicoId}")]
        public IActionResult IniciarOrden(int id, int mecanicoId)
        {
            if (mecanicoId <= 0)
                return BadRequest("Debe indicar un mecánico válido.");

            try
            {
                _gestorOrdenes.InicieOrdenDeTrabajo(id, mecanicoId);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
