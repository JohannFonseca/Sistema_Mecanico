using SistemaOrdenesReparacion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenesReparacion.BL
{
    public interface IGestorDeOrdenesEnProceso
    {
        List<Model.OrdenDeTrabajo> ObtengaLasOrdenesEnProceso();
        Model.OrdenDeTrabajo ObtengaOrdenPorId(int id);
        void AgregarRepuestoAOrden(int idOrden, int idRepuesto, int cantidad);
        List<OrdenRepuesto> ObtenerRepuestosAsignadosAOrden(int idOrden);
        void AgregarServicioAOrden(int idOrden, int idServicio, int cantidad);
        List<OrdenServicio> ObtenerServiciosAsignadosAOrden(int idOrden);
        ResumenTotal ObtenerTotalesDeOrden(int idOrden);

    }
}
