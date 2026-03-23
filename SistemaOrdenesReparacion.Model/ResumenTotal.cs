using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenesReparacion.Model
{
    public class ResumenTotal
    {
        public decimal TotalRepuestos { get; set; }
        public decimal TotalServicios { get; set; }
        public decimal TotalGeneral => TotalRepuestos + TotalServicios;
    }

}
