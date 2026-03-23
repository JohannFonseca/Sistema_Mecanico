using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaOrdenesReparacion.Model;
using System.Net.Http.Json;

namespace SistemaOrdenesReparacion.UI.Controllers
{
    [Authorize(Roles = "Mecanico")]
    public class OrdenesEnProcesoController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public OrdenesEnProcesoController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Index(string nombre)
        {
            List<OrdenDeTrabajo> ordenesEnProceso;
            try
            {
                var response = await _client.GetAsync("api/OrdenesEnProceso/ObtengaLasOrdenesEnProceso");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                ordenesEnProceso = JsonConvert.DeserializeObject<List<OrdenDeTrabajo>>(content);
                ViewData["ProblemasAlObtener"] = false;
            }
            catch
            {
                ordenesEnProceso = new List<OrdenDeTrabajo>();
                ViewData["ProblemasAlObtener"] = true;
            }

            ViewData["FiltroCliente"] = nombre;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return View(ordenesEnProceso);
            }

            var ListaFiltrada = ordenesEnProceso
                .Where(r => r.Cliente != null &&
                            r.Cliente.Nombre != null &&
                            r.Cliente.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return View(ListaFiltrada);
        }


        [HttpGet]
        public async Task<IActionResult> AgregarRepuesto(int id)
        {
            var repuestos = await _client.GetFromJsonAsync<List<InventarioDeRepuesto>>("api/Repuestos/ObtenerRepuestos") ?? new List<InventarioDeRepuesto>();
            ViewBag.Repuestos = repuestos;
            ViewBag.IdOrden = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarRepuesto(int idOrden, int idRepuesto, int cantidad)
        {
            if (cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor que cero.");

            try
            {
                var repuesto = new RepuestoParaOrdenEnProceso
                {
                    IdOrden = idOrden,
                    IdRepuesto = idRepuesto,
                    Cantidad = cantidad
                };

                var response = await _client.PostAsJsonAsync("api/OrdenesEnProceso/AgregarRepuesto", repuesto);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("VerRepuestos", new { id = idOrden });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return BadRequest($"Error desde la API: {error}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al agregar repuesto: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> VerRepuestos(int id)
        {
            var repuestos = await _client.GetFromJsonAsync<List<OrdenRepuesto>>(
                $"api/OrdenesEnProceso/ObtenerRepuestosAsignadosAOrden/{id}")
                ?? new List<OrdenRepuesto>();

            return View(repuestos);
        }


        [HttpGet]
        public async Task<IActionResult> AgregarServicio(int id)
        {
            var Servicios = await _client.GetFromJsonAsync<List<InventarioDeRepuesto>>("api/Servicios/MuestreLaListaDeServicios") ?? new List<InventarioDeRepuesto>();
            ViewBag.Servicios = Servicios;
            ViewBag.IdOrden = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarServicio(int idOrden, int idServicio, int cantidad)
        {
            if (cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor que cero.");

            try
            {
                var servicio = new ServicioParaOrdenEnProceso
                {
                    IdOrden = idOrden,
                    IdServicio = idServicio,
                    Cantidad = cantidad
                };

                var response = await _client.PostAsJsonAsync("api/OrdenesEnProceso/AgregarServicioAOrden", servicio);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("VerServicios", new { id = idOrden });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return BadRequest($"Error desde la API: {error}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al agregar servicio: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerServicios(int id)
        {
            var Servicios = await _client.GetFromJsonAsync<List<OrdenServicio>>(
                $"api/OrdenesEnProceso/ObtenerServiciosAsignadosAOrden/{id}")
                ?? new List<OrdenServicio>();

            return View(Servicios);
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> VerTotal(int id)
        {
            var resumen = await _client.GetFromJsonAsync<ResumenTotal>($"api/OrdenesEnProceso/ObtenerTotalesDeOrden?id={id}");
            return View(resumen);
        }

    }
}
