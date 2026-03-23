using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaOrdenesReparacion.Model;
using System.Net.Http.Json;

namespace SistemaOrdenesReparacion.UI.Controllers
{
    public class OrdenesCanceladasController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public OrdenesCanceladasController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filtroCliente = null)
        {
            string url = "api/ordenes/canceladas";
            if (!string.IsNullOrEmpty(filtroCliente))
                url += $"?filtroCliente={filtroCliente}";

            var lista = await _client.GetFromJsonAsync<List<OrdenDeTrabajo>>(url) ?? new List<OrdenDeTrabajo>();

            ViewData["FiltroCliente"] = filtroCliente;
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var orden = await _client.GetFromJsonAsync<OrdenDeTrabajo>($"api/ordenes/canceladas/{id}");
            if (orden == null)
                return NotFound();

            return View(orden);
        }
    }
}
