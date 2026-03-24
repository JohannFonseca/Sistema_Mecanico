using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Taller_Mecanico_de_Citas.Model
{
    public class Login
    {
        [Required(ErrorMessage = "El usuario es requerido.")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "La clave es requerida")]
        [DataType(DataType.Password)]
        public string clave { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public int Rol { get; set; }
    }
}

