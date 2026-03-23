using System.ComponentModel.DataAnnotations;

namespace SistemaOrdenesReparacion.Model
{
    public class CambioClave
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La clave actual es obligatoria.")]
        [DataType(DataType.Password)]
        public string ClaveActual { get; set; }

        [Required(ErrorMessage = "La nueva clave es obligatoria.")]
        [DataType(DataType.Password)]
        public string NuevaClave { get; set; }
    }
}
