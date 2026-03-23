using Microsoft.AspNetCore.Mvc;
using SistemaOrdenesReparacion.BL;
using SistemaOrdenesReparacion.Model;
using System.Collections.Generic;

namespace SistemaOrdenesReparacion.SI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly IGestorInventarioDeServicios _gestor;

        public ServiciosController(IGestorInventarioDeServicios gestor)
        {
            _gestor = gestor;
        }

        // GET: api/servicios
        [HttpGet("MuestreLaListaDeServicios")]
        public List<Model.InventarioDeServicios> MuestreLaListaDeServicios()
        {
            return _gestor.MuestreLaListaDeServicios();
        }

        // GET: api/servicios
        [HttpGet("ObtengaServicioPorId/{id}")]
        public Model.InventarioDeServicios ObtengaServicioPorId(int id)
        {
            return _gestor.ObtengaServicioPorId(id);
        }

        [HttpPost("AgregueServicio")]
        public void AgregueServicio([FromBody] InventarioDeServicios servicio)
        {
            _gestor.AgregueServicio(servicio);
        }

        [HttpPut("EditeUnServicio/{id}")]
        public void EditeUnServicio(int id, [FromBody] Model.InventarioDeServicios servicio)
        {
            _gestor.EditeUnServicio(servicio.Nombre, servicio.Descripcion, servicio.Precio, id);
        }
    }
}


