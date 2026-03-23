using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenesReparacion.Model
{
    public class OrdenServicio
    {
        public int Id { get; set; }
        public int Id_OrdenesDeTrabajo { get; set; }
        public int Id_InventarioDeServicios { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }

        public OrdenDeTrabajo OrdenDeTrabajo { get; set; }
        public InventarioDeServicios Servicio { get; set; }
    }

}
