using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaOrdenesReparacion.Model
{
    [Table("Mecanicos")]
    public class Mecanico
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        public int Identificacion { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido.")]
        public string CorreoElectronico { get; set; }

        public Usuario? Usuario { get; set; }

        public ICollection<OrdenDeTrabajo> OrdenesAsignadas { get; set; } = new List<OrdenDeTrabajo>();
    }
}
