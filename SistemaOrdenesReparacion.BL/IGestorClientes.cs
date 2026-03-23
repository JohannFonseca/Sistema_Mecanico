using SistemaOrdenesReparacion.Model;
using System.Collections.Generic;

namespace SistemaOrdenesReparacion.BL
{
    public interface IGestorClientes
    {
        void AgregueCliente(Cliente cliente);
        void EditeCliente(Cliente cliente);
        Cliente? ObtengaClientePorId(int id);
        List<Cliente> MuestreLaListaDeClientes(string? filtro);
    }
}

