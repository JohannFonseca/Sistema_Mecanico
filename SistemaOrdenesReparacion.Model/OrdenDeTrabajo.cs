using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SistemaOrdenesReparacion.Model
{
    public class OrdenDeTrabajo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Id_Cliente es obligatorio. Por favor, ingrese un valor.")]
        public int Id_Cliente { get; set; }

        [ForeignKey("Id_Cliente")]
        public Cliente? Cliente { get; set; }

        [Required(ErrorMessage = "El campo Fecha de registro es obligatorio. Debe especificar la fecha.")]
        public DateTime FechaDeRegistro { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El campo Fecha aproximada de finalización es obligatorio. Indique la fecha prevista.")]
        public DateTime FechaAproximadaDeFinalizacion { get; set; }

        [Required(ErrorMessage = "El estado de la orden es obligatorio. Seleccione una opción.")]
        public EstadoOrden Estado { get; set; } = EstadoOrden.Registrado;

        public int? Id_Mecanico { get; set; }

        [ForeignKey("Id_Mecanico")]
        public Mecanico? Mecanico { get; set; }

        [Required(ErrorMessage = "El tipo de vehículo es obligatorio. Elija un tipo.")]
        public TipoVehiculo TipoDeVehiculo { get; set; }

        [Required(ErrorMessage = "El tipo de combustión es obligatorio. Seleccione el tipo correspondiente.")]
        public TipoCombustion TipoDeCombustionInterna { get; set; }

        [Required(ErrorMessage = "La marca del vehículo es obligatoria. Ingrese la marca.")]
        public string? Marca { get; set; }

        [Required(ErrorMessage = "El modelo del vehículo es obligatorio. Ingrese el modelo.")]
        public string? Modelo { get; set; }

        [Required(ErrorMessage = "El año de fabricación es obligatorio. Indique el año.")]
        public int AñoDeFabricacion { get; set; }

        [Required(ErrorMessage = "La descripción del problema es obligatoria. Describa el problema.")]
        public string? DescripcionDelProblema { get; set; }

        public string? MotivoDeCancelacion { get; set; }

        public ICollection<OrdenRepuesto> Repuestos { get; set; } = new List<OrdenRepuesto>();
        public ICollection<OrdenServicio> Servicios { get; set; } = new List<OrdenServicio>();

        // Propiedad auxiliar para mostrar el texto del enum con el Display(Name = ...)
        public string TipoDeVehiculoTexto
        {
            get
            {
                var tipo = this.TipoDeVehiculo;
                var attr = tipo.GetType()
                    .GetField(tipo.ToString())
                    ?.GetCustomAttribute<DisplayAttribute>();

                return attr == null ? tipo.ToString() : attr.Name;
            }
        }
        public string EstadoTexto
        {
            get
            {
                var tipo = this.Estado;
                var attr = tipo.GetType()
                    .GetField(tipo.ToString())
                    ?.GetCustomAttribute<DisplayAttribute>();
                return attr == null ? tipo.ToString() : attr.Name;
            }
        }
    }
}