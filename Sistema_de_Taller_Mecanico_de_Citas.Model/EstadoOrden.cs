using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Taller_Mecanico_de_Citas.Model
{
    public enum EstadoOrden
    {
        [Display(Name = "Registrado")]
        Registrado = 1,

        [Display(Name = "En Proceso")]
        EnProceso = 2,

        [Display(Name = "Finalizado")]
        Finalizado = 3,

        [Display(Name = "Cancelado")]
        Cancelado = 4,

        [Display(Name = "Facturado/Pagado")]
        Facturado = 5
    }
}
