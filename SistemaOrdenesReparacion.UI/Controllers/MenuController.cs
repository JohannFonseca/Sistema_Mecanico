using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace SistemaOrdenesReparacion.UI.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY_HEADER = "x-api-key";
        private const string API_KEY = "J!oA94xr@2Lq8wZf";

        public MenuController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("API");
            if (!_client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                _client.DefaultRequestHeaders.Add(API_KEY_HEADER, API_KEY);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("api/Menu/GetMenuInfo");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var menuInfo = JsonConvert.DeserializeObject<dynamic>(content);

                ViewData["NombreUsuario"] = menuInfo?.NombreUsuario;
                ViewData["Rol"] = menuInfo?.Rol;
                ViewData["ProblemasAlObtener"] = false;

                return View(); 
            }
            catch (Exception)
            {
                ViewData["ProblemasAlObtener"] = true;
                return View(); 
            }
        }
    }

}



