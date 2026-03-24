using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Taller_Mecanico_de_Citas.BL
{
    public interface IGestorDeRepuestos
    {
        List<Model.InventarioDeRepuesto> ObtenerRepuestos();
        Model.InventarioDeRepuesto ObtenerRepuestoPorId(int id);
        void AgregarRepuesto(Model.InventarioDeRepuesto repuesto);
        void EditeUnRepuesto(string nombre, string descripcion, int precio, int id);
    }
}
