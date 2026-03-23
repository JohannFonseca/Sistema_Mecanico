using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaOrdenesReparacion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaOrdenesReparacion.UI.Controllers
{
    [Authorize(Roles = "Mecanico")]
    public class OrdenesDeTrabajoController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public OrdenesDeTrabajoController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }
        public async Task<IActionResult> Index(string filtroCliente)
        {
            List<OrdenDeTrabajo> listaDeOrdenes;
            try
            {
                var url = "api/OrdenesDeTrabajo/MuestreLaListaDeOrdenes";
                if (!string.IsNullOrWhiteSpace(filtroCliente))
                    url += $"?filtroCliente={Uri.EscapeDataString(filtroCliente)}";

                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                listaDeOrdenes = JsonConvert.DeserializeObject<List<OrdenDeTrabajo>>(content);

                // Filtrar para mostrar solo órdenes con estado Registrado
                listaDeOrdenes = listaDeOrdenes.Where(o => o.Estado == EstadoOrden.Registrado).ToList();

                ViewData["ProblemasAlObtener"] = false;
            }
            catch (Exception)
            {
                listaDeOrdenes = new List<OrdenDeTrabajo>();
                ViewData["ProblemasAlObtener"] = true;
            }

            return View(listaDeOrdenes);
        }


        public async Task<IActionResult> Create()
        {
            try
            {
                await CargarDatosViewBag();
                var nuevaOrden = new OrdenDeTrabajo
                {
                    FechaAproximadaDeFinalizacion = DateTime.Today.AddDays(1)
                };
                return View(nuevaOrden);
            }
            catch
            {
                ViewBag.Clientes = new List<object>();
                ViewBag.TiposVehiculo = new List<SelectListItem>();
                ViewBag.TiposCombustion = new List<SelectListItem>();
                return View(new OrdenDeTrabajo());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrdenDeTrabajo orden)
        {
            if (!ModelState.IsValid)
            {
                await CargarDatosViewBag();
                return View(orden);
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(orden);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("api/OrdenesDeTrabajo/AgregueOrdenDeTrabajo", content);
                response.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await CargarDatosViewBag();
                return View(orden);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            OrdenDeTrabajo orden;
            try
            {
                var response = await _client.GetAsync("api/OrdenesDeTrabajo/MuestreLaListaDeOrdenes");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var lista = JsonConvert.DeserializeObject<List<OrdenDeTrabajo>>(content);
                orden = lista.FirstOrDefault(o => o.Id == id);

                if (orden == null) return NotFound();

                await CargarDatosViewBag();
                ViewData["ProblemasAlEditar"] = false;
                return View(orden);
            }
            catch (Exception)
            {
                ViewData["ProblemasAlEditar"] = true;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrdenDeTrabajo orden)
        {
            if (!ModelState.IsValid)
            {
                await CargarDatosViewBag();
                return View(orden);
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(orden);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"api/OrdenesDeTrabajo/EditeOrdenDeTrabajo/{orden.Id}", content);
                response.EnsureSuccessStatusCode();
                ViewData["ProblemasAlEditar"] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await CargarDatosViewBag();
                ViewData["ProblemasAlEditar"] = true;
                return View(orden);
            }
        }

        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null) return NotFound();

            OrdenDeTrabajo orden;
            try
            {
                var response = await _client.GetAsync("api/OrdenesDeTrabajo/MuestreLaListaDeOrdenes");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var lista = JsonConvert.DeserializeObject<List<OrdenDeTrabajo>>(content);
                orden = lista.FirstOrDefault(o => o.Id == id);

                if (orden == null) return NotFound();

                ViewData["ProblemasAlCancelar"] = false;
                return View(orden);
            }
            catch (Exception)
            {
                ViewData["ProblemasAlCancelar"] = true;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(OrdenDeTrabajo orden)
        {
            if (string.IsNullOrWhiteSpace(orden.MotivoDeCancelacion))
            {
                ModelState.AddModelError(nameof(orden.MotivoDeCancelacion), "Debe indicar el motivo de cancelación.");
                return View(orden);
            }

            try
            {
                var url = $"api/OrdenesDeTrabajo/CanceleOrdenDeTrabajo/{orden.Id}?motivoCancelacion={Uri.EscapeDataString(orden.MotivoDeCancelacion)}";
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                ViewData["ProblemasAlCancelar"] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewData["ProblemasAlCancelar"] = true;
                return View(orden);
            }
        }

        public async Task<IActionResult> IniciarOrden(int? id)
        {
            if (id == null) return NotFound();

            OrdenDeTrabajo orden;
            try
            {
                var responseOrden = await _client.GetAsync("api/OrdenesDeTrabajo/MuestreLaListaDeOrdenes");
                responseOrden.EnsureSuccessStatusCode();
                var contentOrden = await responseOrden.Content.ReadAsStringAsync();
                var listaOrdenes = JsonConvert.DeserializeObject<List<OrdenDeTrabajo>>(contentOrden);
                orden = listaOrdenes.FirstOrDefault(o => o.Id == id);

                if (orden == null) return NotFound();

                await CargarMecanicosViewBag();  // Carga los mecánicos igual que los clientes

                ViewData["ProblemasAlIniciar"] = false;
                return View(orden);
            }
            catch (Exception)
            {
                ViewData["ProblemasAlIniciar"] = true;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarOrden(int id, int? mecanicoId)
        {
            if (!mecanicoId.HasValue)
            {
                ModelState.AddModelError("mecanicoId", "Debe seleccionar un mecánico.");

                await CargarMecanicosViewBag();

                var response = await _client.GetAsync("api/OrdenesDeTrabajo/MuestreLaListaDeOrdenes");
                var content = await response.Content.ReadAsStringAsync();
                var lista = JsonConvert.DeserializeObject<List<OrdenDeTrabajo>>(content);
                var orden = lista.FirstOrDefault(o => o.Id == id);
                return View(orden);
            }

            try
            {
                // ✅ CORREGIDO: pasar mecanicoId como parte de la ruta y enviar un cuerpo vacío
                var url = $"api/OrdenesDeTrabajo/IniciarOrden/{id}/{mecanicoId.Value}";
                var requestContent = new StringContent("", Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(url, requestContent);
                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await CargarMecanicosViewBag();

                var response = await _client.GetAsync("api/OrdenesDeTrabajo/MuestreLaListaDeOrdenes");
                var content = await response.Content.ReadAsStringAsync();
                var lista = JsonConvert.DeserializeObject<List<OrdenDeTrabajo>>(content);
                var orden = lista.FirstOrDefault(o => o.Id == id);
                return View(orden);
            }
        }


        private async Task CargarDatosViewBag()
        {
            try
            {
                var responseClientes = await _client.GetAsync("api/clientes");
                responseClientes.EnsureSuccessStatusCode();
                var contentClientes = await responseClientes.Content.ReadAsStringAsync();
                var listaClientes = JsonConvert.DeserializeObject<List<Cliente>>(contentClientes);

                ViewBag.Clientes = listaClientes
                    .Select(c => new {
                        Id = c.Id,
                        NombreCompleto = $"{c.Identificacion} - {c.Nombre} {c.Apellidos}"
                    }).ToList();
            }
            catch
            {
                ViewBag.Clientes = new List<object>();
            }

            ViewBag.TiposVehiculo = Enum.GetValues(typeof(TipoVehiculo))
                .Cast<TipoVehiculo>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();

            ViewBag.TiposCombustion = Enum.GetValues(typeof(TipoCombustion))
                .Cast<TipoCombustion>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();
        }

        private async Task CargarMecanicosViewBag()
        {
            try
            {
                var responseMecanicos = await _client.GetAsync("api/mecanicos");
                responseMecanicos.EnsureSuccessStatusCode();
                var contentMecanicos = await responseMecanicos.Content.ReadAsStringAsync();

                var listaMecanicos = JsonConvert.DeserializeObject<List<Mecanico>>(contentMecanicos);

                ViewBag.Mecanicos = listaMecanicos
                    .Select(m => new {
                        Id = m.Id,
                        NombreCompleto = $"{m.Nombre} {m.Apellidos}"
                    })
                    .ToList();
            }
            catch
            {
                ViewBag.Mecanicos = new List<object>();
            }
        }
    }
}
