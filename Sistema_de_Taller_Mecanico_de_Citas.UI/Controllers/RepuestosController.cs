using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System.Text;

namespace Sistema_de_Taller_Mecanico_de_Citas.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RepuestosController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public RepuestosController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        // GET: Repuestos
        public async Task<IActionResult> Index(string nombre)
        {
            List<InventarioDeRepuesto> ListaDeRepuestos;
            try
            {
                var response = await _client.GetAsync("api/Repuestos/ObtenerRepuestos");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                ListaDeRepuestos = JsonConvert.DeserializeObject<List<InventarioDeRepuesto>>(content);
                ViewData["ProblemasAlObtener"] = false;
            }
            catch
            {
                ListaDeRepuestos = new List<InventarioDeRepuesto>();
                ViewData["ProblemasAlObtener"] = true;
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return View(ListaDeRepuestos);
            }
            else
            {
                var ListaFiltrada = ListaDeRepuestos
                    .Where(r => r.Nombre != null && r.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                return View(ListaFiltrada);
            }
        }

        // GET: Repuestos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Repuestos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventarioDeRepuesto repuesto)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(repuesto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("api/Repuestos/AgregarRepuesto", content);
                response.EnsureSuccessStatusCode();
                ViewData["ProblemasAlCrear"] = false;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["ProblemasAlCrear"] = true;
                return View();
            }
        }

        // GET: Repuestos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            InventarioDeRepuesto repuesto;
            try
            {
                var response = await _client.GetAsync($"api/Repuestos/ObtenerRepuestoPorId/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                repuesto = JsonConvert.DeserializeObject<InventarioDeRepuesto>(content);
                ViewData["ProblemasAlEditar"] = false;
                return View(repuesto);
            }
            catch
            {
                ViewData["ProblemasAlEditar"] = true;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Repuestos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InventarioDeRepuesto repuesto)
        {
            if (!ModelState.IsValid)
                return View(repuesto);

            try
            {
                var jsonContent = JsonConvert.SerializeObject(repuesto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"api/Repuestos/EditeUnRepuesto/{repuesto.Id}", content);
                response.EnsureSuccessStatusCode();
                ViewData["ProblemasAlEditar"] = false;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["ProblemasAlEditar"] = true;
                return View(repuesto);
            }
        }
    }
}
