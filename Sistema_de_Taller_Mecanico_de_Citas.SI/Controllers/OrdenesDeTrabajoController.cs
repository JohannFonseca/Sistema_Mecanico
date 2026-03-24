using Microsoft.AspNetCore.Mvc;
using Sistema_de_Taller_Mecanico_de_Citas.BL;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_de_Taller_Mecanico_de_Citas.SI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenesDeTrabajoController : ControllerBase
    {
        private readonly IGestorOrdenesDeTrabajo _gestorOrdenes;
        private readonly IGestorMecanicos _gestorMecanicos;
        private readonly GestorEmail _gestorEmail;

        public OrdenesDeTrabajoController(IGestorOrdenesDeTrabajo gestorOrdenes, IGestorMecanicos gestorMecanicos, GestorEmail gestorEmail)
        {
            _gestorOrdenes = gestorOrdenes;
            _gestorMecanicos = gestorMecanicos;
            _gestorEmail = gestorEmail;
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

        [HttpPost("FinalizarOrden")]
        public async Task<IActionResult> FinalizarOrden([FromBody] OrdenDeTrabajo model)
        {
            try
            {
                var orden = _gestorOrdenes.ObtenerOrdenPorId(model.Id);
                if (orden == null) return NotFound("Orden no encontrada");

                orden.MetodoPago = model.MetodoPago;
                orden.Pagado = true;
                orden.Estado = EstadoOrden.Facturado;
                orden.FechaDeEntregaReal = System.DateTime.Now;

                // Calcular Total
                decimal total = 0;
                string detalleHtml = "";
                foreach (var s in orden.Servicios)
                {
                    total += (decimal)s.Total;
                    detalleHtml += $"<tr><td style='padding:10px; border-bottom:1px solid #eee;'>{s.Servicio.Nombre}</td><td style='padding:10px; border-bottom:1px solid #eee; text-align:right;'>₡{s.Total:N0}</td></tr>";
                }
                foreach (var r in orden.Repuestos)
                {
                    total += (decimal)r.Total;
                    detalleHtml += $"<tr><td style='padding:10px; border-bottom:1px solid #eee;'>{r.Repuesto.Nombre} x {r.Cantidad}</td><td style='padding:10px; border-bottom:1px solid #eee; text-align:right;'>₡{r.Total:N0}</td></tr>";
                }

                orden.MontoTotal = total;
                _gestorOrdenes.EditeOrdenDeTrabajo(orden);

                // Enviar Factura
                if (orden.Cliente != null && !string.IsNullOrEmpty(orden.Cliente.CorreoElectronico))
                {
                    await _gestorEmail.EnviarCorreoFacturaAsync(
                        orden.Cliente.CorreoElectronico,
                        $"{orden.Cliente.Nombre} {orden.Cliente.Apellidos}",
                        orden.Id.ToString(),
                        detalleHtml,
                        total,
                        orden.MetodoPago
                    );
                }

                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
