using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SistemaOrdenesReparacion.UI.Helpers
{
    public static class EnumHelper
    {
        public static string ObtenerNombreDisplay(Enum valor)
        {
            var tipo = valor.GetType();
            var miembro = tipo.GetMember(valor.ToString()).FirstOrDefault();
            var atributo = miembro?.GetCustomAttribute<DisplayAttribute>();
            return atributo?.Name ?? valor.ToString();
        }
    }
}
