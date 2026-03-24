using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System.Collections.Generic;

namespace Sistema_de_Taller_Mecanico_de_Citas.BL
{
    public interface IGestorInventarioDeServicios
    {
        List<Model.InventarioDeServicios> MuestreLaListaDeServicios();
        Model.InventarioDeServicios ObtengaServicioPorId(int id);
        void AgregueServicio(Model.InventarioDeServicios servicio);
        void EditeUnServicio(string nombre, string descripcion, int precio, int id);

    }
}