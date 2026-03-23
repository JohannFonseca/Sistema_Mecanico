using Microsoft.AspNetCore.Mvc;
using SistemaOrdenesReparacion.Model;
using System.Net.Http.Json;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SistemaOrdenesReparacion.UI.Controllers
{
    public class OrdenesClienteController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public OrdenesClienteController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        public async Task<IActionResult> Index(string? estado = null)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int clienteId) || clienteId <= 0)
            {
                ViewBag.Error = "No se pudo obtener el Id del cliente desde el usuario autenticado.";
                return View(new List<OrdenDeTrabajo>());
            }

            string url = $"api/ordenes/cliente?idCliente={clienteId}";

            if (!string.IsNullOrEmpty(estado))
                url += $"&estado={estado}";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error al obtener órdenes: {response.StatusCode}";
                return View(new List<OrdenDeTrabajo>());
            }

            var ordenes = await response.Content.ReadFromJsonAsync<List<OrdenDeTrabajo>>();
            if (ordenes == null) ordenes = new List<OrdenDeTrabajo>();

            ViewBag.EstadoSeleccionado = estado;

            ViewBag.Estados = Enum.GetValues(typeof(EstadoOrden))
                .Cast<EstadoOrden>()
                .Select(e => new
                {
                    Valor = e.GetType()
                             .GetField(e.ToString())
                             ?.GetCustomAttribute<DisplayAttribute>()
                             ?.Name ?? e.ToString(),
                    Texto = e.GetType()
                             .GetField(e.ToString())
                             ?.GetCustomAttribute<DisplayAttribute>()
                             ?.Name ?? e.ToString()
                }).ToList();

            return View(ordenes);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int clienteId) || clienteId <= 0)
            {
                ViewBag.Error = "No se pudo obtener el Id del cliente desde el usuario autenticado.";
                return RedirectToAction(nameof(Index));
            }

            string url = $"api/ordenes/cliente/{id}?idCliente={clienteId}";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Error al obtener detalles: {response.StatusCode}";
                return RedirectToAction(nameof(Index));
            }

            var json = await response.Content.ReadAsStringAsync();

            JObject jobject;
            try
            {
                jobject = JObject.Parse(json);
            }
            catch
            {
                ViewBag.Error = "Error al interpretar la respuesta del servidor.";
                return RedirectToAction(nameof(Index));
            }

            if (jobject["orden"] == null)
            {
                ViewBag.Error = "No se encontró la orden solicitada en la respuesta.";
                return RedirectToAction(nameof(Index));
            }

            var orden = jobject["orden"].ToObject<OrdenDeTrabajo>();

            var repuestos = jobject["repuestos"]?.ToObject<List<OrdenRepuesto>>() ?? new List<OrdenRepuesto>();

            var servicios = new List<OrdenServicio>();
            if (jobject["servicios"] != null)
            {
                foreach (var item in jobject["servicios"])
                {
                    var ordenServicio = item.ToObject<OrdenServicio>();

                    if (item["servicio"] != null)
                        ordenServicio.Servicio = item["servicio"].ToObject<InventarioDeServicios>();

                    servicios.Add(ordenServicio);
                }
            }

            ViewBag.Repuestos = repuestos;
            ViewBag.Servicios = servicios;
            ViewBag.TotalRepuestos = jobject["totalRepuestos"]?.Value<decimal>() ?? 0m;
            ViewBag.TotalServicios = jobject["totalServicios"]?.Value<decimal>() ?? 0m;
            ViewBag.TotalOrden = jobject["totalOrden"]?.Value<decimal>() ?? 0m;

            return View(orden);
        }
    }
}
