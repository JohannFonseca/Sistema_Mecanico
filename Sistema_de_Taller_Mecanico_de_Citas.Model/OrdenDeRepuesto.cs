using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Taller_Mecanico_de_Citas.Model
{
    public class OrdenRepuesto
    {
        public int Id { get; set; }
        public int Id_OrdenesDeTrabajos { get; set; }
        public int Id_InventarioDeRepuestos { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }

        public OrdenDeTrabajo OrdenDeTrabajo { get; set; }
        public InventarioDeRepuesto Repuesto { get; set; }
    }

}
