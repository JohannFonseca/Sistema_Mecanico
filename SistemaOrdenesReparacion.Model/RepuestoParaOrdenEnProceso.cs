using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenesReparacion.Model
{
    public class RepuestoParaOrdenEnProceso
    {
        public int IdOrden { get; set; }
        public int IdRepuesto { get; set; }
        public int Cantidad { get; set; }
    }
}
