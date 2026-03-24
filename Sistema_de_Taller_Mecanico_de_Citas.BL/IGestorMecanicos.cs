using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System.Collections.Generic;

namespace Sistema_de_Taller_Mecanico_de_Citas.BL
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

