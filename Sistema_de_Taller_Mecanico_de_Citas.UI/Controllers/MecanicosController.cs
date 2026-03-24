using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System.Net.Http.Json;

namespace Sistema_de_Taller_Mecanico_de_Citas.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MecanicosController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public MecanicosController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");

            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        public async Task<IActionResult> Index(string filtroNombre)
        {
            var response = await _client.GetAsync("api/mecanicos");

            if (!response.IsSuccessStatusCode)
                return Content("Error al obtener la lista de mecánicos.");

            var mecanicos = await response.Content.ReadFromJsonAsync<List<Mecanico>>();

            if (!string.IsNullOrEmpty(filtroNombre))
            {
                filtroNombre = filtroNombre.ToLower();
                mecanicos = mecanicos
                    .Where(m => m.Nombre != null && m.Nombre.ToLower().Contains(filtroNombre))
                    .ToList();
            }

            ViewData["FiltroNombre"] = filtroNombre;
            return View(mecanicos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Mecanico mecanico)
        {
            if (!ModelState.IsValid)
                return View(mecanico);

            var response = await _client.PostAsJsonAsync("api/mecanicos", mecanico);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Error al crear mecánico: {error}");

            return View(mecanico);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var response = await _client.GetAsync($"api/mecanicos/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound("Mecánico no encontrado.");

            var mecanico = await response.Content.ReadFromJsonAsync<Mecanico>();

            if (mecanico == null)
                return NotFound("Mecánico no encontrado.");

            var viewModel = new EditarMecanicoViewModel
            {
                Id = mecanico.Id,
                Identificacion = mecanico.Identificacion,
                Nombre = mecanico.Nombre,
                Apellidos = mecanico.Apellidos
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EditarMecanicoViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            // Recuperar el mecanico completo del API para no perder campos no editables (correo)
            var response = await _client.GetAsync($"api/mecanicos/{modelo.Id}");
            if (!response.IsSuccessStatusCode)
                return NotFound("Mecánico no encontrado.");

            var mecanico = await response.Content.ReadFromJsonAsync<Mecanico>();
            if (mecanico == null)
                return NotFound("Mecánico no encontrado.");

            // Actualizar solo campos permitidos desde el ViewModel
            mecanico.Nombre = modelo.Nombre;
            mecanico.Apellidos = modelo.Apellidos;
            mecanico.Identificacion = modelo.Identificacion;

            var updateResponse = await _client.PutAsJsonAsync($"api/mecanicos/{modelo.Id}", mecanico);
            if (!updateResponse.IsSuccessStatusCode)
            {
                var error = await updateResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Error al actualizar: {error}");
                return View(modelo);
            }

            return RedirectToAction("Index");
        }

    }
}
