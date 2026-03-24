using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Taller_Mecanico_de_Citas.Model
{
    public class ServicioParaOrdenEnProceso
    {
        public int IdOrden { get; set; }
        public int IdServicio { get; set; }
        public int Cantidad { get; set; }
    }
}
