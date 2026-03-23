using SistemaOrdenes.Model;

namespace SistemaOrdenes.BL
{
    public interface ICatalogoOrdenes
    {
        void AgregarOrden(OrdenDeReparacion orden);
        void EditarOrden(int id, OrdenDeReparacion datosEditados);
        void EliminarOrden(int id);
        List<OrdenDeReparacion> ObtenerOrdenes();
        OrdenDeReparacion? ObtenerPorId(int id);
    }
}

