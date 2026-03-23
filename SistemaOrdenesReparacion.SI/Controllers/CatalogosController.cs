using Microsoft.AspNetCore.Mvc;
using SistemaOrdenesReparacion.Model;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

[ApiController]
[Route("api/catalogos")]
public class CatalogosController : ControllerBase
{
    [HttpGet("tiposvehiculo")]
    public IActionResult GetTiposVehiculo()
    {
        var lista = Enum.GetValues(typeof(TipoVehiculo))
            .Cast<TipoVehiculo>()
            .Select(tv => new {
                Value = (int)tv,
                Text = tv.GetType()
                    .GetMember(tv.ToString())[0]
                    .GetCustomAttribute<DisplayAttribute>()?.Name ?? tv.ToString()
            }).ToList();

        return Ok(lista);
    }

    [HttpGet("tiposcombustion")]
    public IActionResult GetTiposCombustion()
    {
        var lista = Enum.GetValues(typeof(TipoCombustion))
            .Cast<TipoCombustion>()
            .Select(tc => new {
                Value = (int)tc,
                Text = tc.GetType()
                    .GetMember(tc.ToString())[0]
                    .GetCustomAttribute<DisplayAttribute>()?.Name ?? tc.ToString()
            }).ToList();

        return Ok(lista);
    }
}
