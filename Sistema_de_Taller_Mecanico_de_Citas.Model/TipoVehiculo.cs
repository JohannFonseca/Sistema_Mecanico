using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Taller_Mecanico_de_Citas.Model
{
    public enum TipoVehiculo
    {
        [Display(Name = "Automóvil")]
        Automovil = 0,

        [Display(Name = "SUV")]
        SUV = 1,

        [Display(Name = "Pickup")]
        Pickup = 2,

        [Display(Name = "Autobús")]
        Autobus = 3,

        [Display(Name = "Trolebús")]
        Trolebus = 4,

        [Display(Name = "Microbús")]
        Microbus = 5,

        [Display(Name = "Camión")]
        Camion = 6,

        [Display(Name = "Tráiler")]
        Trailer = 7,

        [Display(Name = "Furgoneta")]
        Furgoneta = 8,

        [Display(Name = "Ambulancia")]
        Ambulancia = 9,

        [Display(Name = "Camión de Bomberos")]
        CamionDeBomberos = 10,

        [Display(Name = "Vehículo Militar")]
        VehiculoMilitar = 11,

        [Display(Name = "Maquinaria Agrícola")]
        MaquinariaAgricola = 12,

        [Display(Name = "Maquinaria de Construcción")]
        MaquinariaConstruccion = 13
    }
}
