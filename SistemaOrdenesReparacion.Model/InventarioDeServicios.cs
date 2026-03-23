using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaOrdenesReparacion.Model
{
    [Table("InventarioDeServicios")]

    public class InventarioDeServicios
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public required string Descripcion { get; set; }

        [Column("Precio", TypeName = "money")]
        [Required(ErrorMessage = "El precio del servicio es obligatorio.")]
        [Display(Name = "Precio Unitario")]
        public int Precio { get; set; }

        public ICollection<OrdenServicio>? Ordenes { get; set; }
    }
}