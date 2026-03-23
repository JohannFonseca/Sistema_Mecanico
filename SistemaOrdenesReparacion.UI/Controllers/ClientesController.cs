using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaOrdenesReparacion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenesReparacion.UI.Controllers
{
    [Authorize(Roles = "Administrador,Mecanico")]
    public class ClientesController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public ClientesController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        // GET: Clientes
        public async Task<IActionResult> Index(string filtro = "")
        {
            try
            {
                var url = $"api/clientes";
                if (!string.IsNullOrEmpty(filtro))
                    url += $"?filtro={Uri.EscapeDataString(filtro)}";

                var listaDeClientes = await _client.GetFromJsonAsync<List<Cliente>>(url) ?? new List<Cliente>();

                ViewBag.FiltroActual = filtro;
                ViewData["ProblemasAlObtener"] = false;

                return View(listaDeClientes);
            }
            catch
            {
                ViewData["ProblemasAlObtener"] = true;
                return View(new List<Cliente>());
            }
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            try
            {
                var response = await _client.PostAsJsonAsync("api/clientes", modelo);
                response.EnsureSuccessStatusCode();

                TempData["Exito"] = "Cliente agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["ProblemasAlCrear"] = true;
                return View(modelo);
            }
        }
        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var cliente = await _client.GetFromJsonAsync<Cliente>($"api/clientes/{id}");
                if (cliente == null)
                    return NotFound();

                var modelo = new EditarClienteViewModel
                {
                    Id = cliente.Id,
                    Identificacion = cliente.Identificacion,
                    Nombre = cliente.Nombre,
                    Apellidos = cliente.Apellidos
                };

                ViewData["ProblemasAlEditar"] = false;
                return View(modelo); // <- ahora sí pasa el tipo correcto a la vista
            }
            catch
            {
                ViewData["ProblemasAlEditar"] = true;
                return View();
            }
        }


        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditarClienteViewModel clienteEditado)
        {
            if (id != clienteEditado.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(clienteEditado);

            try
            {
                var response = await _client.PutAsJsonAsync($"api/clientes/{id}", clienteEditado);
                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["ProblemasAlEditar"] = true;
                return View(clienteEditado);
            }
        }
    }

}
