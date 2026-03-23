using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaOrdenesReparacion.Model;
using System.Collections.Generic;
using System.Text;

namespace SistemaOrdenesReparacion.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class InventarioDeServiciosController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public InventarioDeServiciosController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        // GET: InventarioDeServicios
        public async Task<IActionResult> Index(string nombre)
        {
            List<InventarioDeServicios> listaDeServicios;
            try
            {
                var response = await _client.GetAsync("api/Servicios/MuestreLaListaDeServicios");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                listaDeServicios = JsonConvert.DeserializeObject<List<InventarioDeServicios>>(content);
                ViewData["ProblemasAlObtener"] = false;
            }
            catch (Exception)
            {
                listaDeServicios = new List<InventarioDeServicios>();
                ViewData["ProblemasAlObtener"] = true;
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return View(listaDeServicios);
            }
            else
            {
                var listaFiltrada = listaDeServicios
                    .Where(s => s.Nombre != null && s.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                return View(listaFiltrada);
            }
        }

        // GET: InventarioDeServicios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InventarioDeServicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventarioDeServicios servicio)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(servicio);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("api/Servicios/AgregueServicio", content);
                response.EnsureSuccessStatusCode();
                ViewData["ProblemasAlCrear"] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewData["ProblemasAlCrear"] = true;
                return View();
            }
        }

        // GET: InventarioDeServicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            InventarioDeServicios servicio;
            try
            {
                var response = await _client.GetAsync($"api/Servicios/ObtengaServicioPorId/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                servicio = JsonConvert.DeserializeObject<InventarioDeServicios>(content);
                ViewData["ProblemasAlEditar"] = false;
                return View(servicio);
            }
            catch (Exception)
            {
                ViewData["ProblemasAlEditar"] = true;
                return View();
            }
        }

        // POST: InventarioDeServicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InventarioDeServicios servicio)
        {
            if (!ModelState.IsValid)
            {
                return View(servicio);
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(servicio);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"api/Servicios/EditeUnServicio/{servicio.Id}", content);
                response.EnsureSuccessStatusCode();
                ViewData["ProblemasAlEditar"] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewData["ProblemasAlEditar"] = true;
                return View(servicio);
            }
        }
    }
}
