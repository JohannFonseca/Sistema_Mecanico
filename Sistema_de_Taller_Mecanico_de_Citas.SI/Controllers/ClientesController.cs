using Microsoft.AspNetCore.Mvc;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using Sistema_de_Taller_Mecanico_de_Citas.BL;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_de_Taller_Mecanico_de_Citas.SI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IGestorClientes _gestor;

        public ClientesController(IGestorClientes gestor)
        {
            _gestor = gestor;
        }

        // GET: api/clientes?filtro=...
        [HttpGet]
        public ActionResult<List<Cliente>> GetClientes(string filtro = null)
        {
            var clientes = _gestor.MuestreLaListaDeClientes(filtro);
            return Ok(clientes);
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public ActionResult<Cliente> GetCliente(int id)
        {
            var cliente = _gestor.ObtengaClientePorId(id);
            if (cliente == null)
                return NotFound();
            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public IActionResult CrearCliente([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _gestor.AgregueCliente(cliente);
                return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public IActionResult EditarCliente(int id, [FromBody] EditarClienteViewModel clienteEditado)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clienteOriginal = _gestor.ObtengaClientePorId(id);
            if (clienteOriginal == null)
                return NotFound();

            try
            {
                clienteOriginal.Identificacion = clienteEditado.Identificacion;
                clienteOriginal.Nombre = clienteEditado.Nombre;
                clienteOriginal.Apellidos = clienteEditado.Apellidos;

                _gestor.EditeCliente(clienteOriginal);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }


}

