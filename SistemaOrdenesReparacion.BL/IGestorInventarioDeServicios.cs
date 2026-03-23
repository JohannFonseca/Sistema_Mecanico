using SistemaOrdenesReparacion.Model;
using System.Collections.Generic;

namespace SistemaOrdenesReparacion.BL
{
    public interface IGestorInventarioDeServicios
    {
        List<Model.InventarioDeServicios> MuestreLaListaDeServicios();
        Model.InventarioDeServicios ObtengaServicioPorId(int id);
        void AgregueServicio(Model.InventarioDeServicios servicio);
        void EditeUnServicio(string nombre, string descripcion, int precio, int id);

    }
}