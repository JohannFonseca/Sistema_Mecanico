using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Taller_Mecanico_de_Citas.Model
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [StringLength(255)]
        public string Clave { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido.")]
        [StringLength(100)]
        public string CorreoElectronico { get; set; }

        [Required]
        public int Rol { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime? BloqueadoHasta { get; set; }

        public DateTime? FechaUltimoCambioClave { get; set; }

        public int IntentosFallidos { get; set; } = 0;

        public int? Id_Cliente { get; set; }

        [ForeignKey("Id_Cliente")]
        public Cliente? Cliente { get; set; }

        public int? Id_Mecanico { get; set; }

        [ForeignKey("Id_Mecanico")]
        public Mecanico? Mecanico { get; set; }
    }
}

