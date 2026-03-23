using System.ComponentModel.DataAnnotations;

namespace SistemaOrdenesReparacion.Model
{
    public enum EstadoOrden
    {
        [Display(Name = "Registrado")]
        Registrado = 1,

        [Display(Name = "En Proceso")]
        EnProceso = 2,

        [Display(Name = "Cancelado")]
        Cancelado = 4
    }
}
