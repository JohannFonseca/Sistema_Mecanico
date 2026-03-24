using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System.Collections.Generic;

namespace Sistema_de_Taller_Mecanico_de_Citas.BL
{
    public interface IGestorClientes
    {
        void AgregueCliente(Cliente cliente);
        void EditeCliente(Cliente cliente);
        Cliente? ObtengaClientePorId(int id);
        List<Cliente> MuestreLaListaDeClientes(string? filtro);
    }
}

