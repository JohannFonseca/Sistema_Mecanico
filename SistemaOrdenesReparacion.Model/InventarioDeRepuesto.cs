using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenesReparacion.Model
{
    [Table("InventarioDeRepuestos")]
    public class InventarioDeRepuesto
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "El nombre del repuesto es obligatorio.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "La descripción del repuesto es obligatoria.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }


        [Column("PrecioUnitario", TypeName = "money")]
        [Required(ErrorMessage = "El precio del repuesto es obligatorio.")]
        [Display(Name = "Precio Unitario")]
        public int Precio { get; set; }

        public ICollection<OrdenRepuesto> Ordenes { get; set; } = new List<OrdenRepuesto>();
    }


}
