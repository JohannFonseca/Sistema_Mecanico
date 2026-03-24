using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System.Security.Claims;
using System.Text.Json;

namespace Sistema_de_Taller_Mecanico_de_Citas.UI.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public LoginController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        [HttpGet]
        public IActionResult Index(int rol = 2)
        {
            var model = new Login { Rol = rol };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Ingresar(Login model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var response = await _client.PostAsJsonAsync("api/login", model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View("Index", model);
            }


            var json = await response.Content.ReadAsStringAsync();
            var userObj = JsonDocument.Parse(json).RootElement;

            string idClienteStr = "";

            if (userObj.TryGetProperty("idCliente", out var idClienteProp) && idClienteProp.ValueKind == JsonValueKind.Number)
            {
                idClienteStr = idClienteProp.GetInt32().ToString();
            }
            else if (userObj.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.Number)
            {
                idClienteStr = idProp.GetInt32().ToString();
            }
            else
            {
                idClienteStr = "0"; // o manejar error si prefieres
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, idClienteStr),
                new Claim(ClaimTypes.Name, userObj.GetProperty("usuario").GetString() ?? ""),
                new Claim(ClaimTypes.Role, userObj.GetProperty("nombreRol").GetString() ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CambiarClave()
        {
            return View(new CambioClave());
        }

        [HttpGet]
        public IActionResult RecuperarClave()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarClave(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                ModelState.AddModelError("", "El usuario es obligatorio.");
                return View();
            }

            var response = await _client.PostAsJsonAsync("api/login/recuperar-clave", usuario);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View();
            }

            TempData["Mensaje"] = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> CambiarClave(CambioClave model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NombreUsuario?.ToLower() == "admin")
            {
                ModelState.AddModelError("", "No está permitido cambiar la contraseña del administrador por este medio.");
                return View(model);
            }

            var response = await _client.PostAsJsonAsync("api/login/cambiar-clave", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View(model);
            }

            TempData["Mensaje"] = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Index");
        }


    }
}
