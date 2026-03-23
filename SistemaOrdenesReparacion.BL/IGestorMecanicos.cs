using SistemaOrdenesReparacion.Model;
using System.Collections.Generic;

namespace SistemaOrdenesReparacion.BL
{
    public interface IGestorMecanicos
    {
        void Agregar(Mecanico mecanico);
        void Editar(Mecanico mecanico);
        void Eliminar(int id);
        Mecanico? ObtenerPorId(int id);
        List<Mecanico> ObtenerTodos();
    }
}

