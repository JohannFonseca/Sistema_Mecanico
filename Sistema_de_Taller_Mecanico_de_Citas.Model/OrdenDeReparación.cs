using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaOrdenes.Model
{
    public class OrdenDeReparacion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [Display(Name = "Nombre del Cliente")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "El nombre del mecánico es obligatorio.")]
        [Display(Name = "Nombre del Mecánico")]
        public string NombreMecanico { get; set; }

        [Required(ErrorMessage = "La descripción del problema es obligatoria.")]
        [Display(Name = "Descripción del Problema")]
        public string DescripcionProblema { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        [Required]
        public DateTime FechaIngreso { get; set; }

        [Display(Name = "Fecha de Entrega Estimada")]
        public DateTime? FechaEntregaEstimada { get; set; }

        [Display(Name = "Estado de la Reparación")]
        public EstadoReparacion Estado { get; set; } = EstadoReparacion.Pendiente;
    }

    public enum EstadoReparacion
    {
        Pendiente = 1,
        EnProceso = 2,
        Finalizado = 4
    }
}
