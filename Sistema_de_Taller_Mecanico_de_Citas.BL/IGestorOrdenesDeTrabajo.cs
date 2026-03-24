using System;
using System.Collections.Generic;
using Sistema_de_Taller_Mecanico_de_Citas.Model;



namespace Sistema_de_Taller_Mecanico_de_Citas.BL
{
    public interface IGestorOrdenesDeTrabajo
    {
        List<OrdenDeTrabajo> MuestreLaListaDeOrdenes(string filtroNombreCliente);

        void AgregueOrdenDeTrabajo(OrdenDeTrabajo orden);

        void EditeOrdenDeTrabajo(OrdenDeTrabajo orden);

        void CanceleOrdenDeTrabajo(int idOrden, string motivoCancelacion);

        void InicieOrdenDeTrabajo(int idOrden, int idMecanico);

        OrdenDeTrabajo ObtenerOrdenPorId(int id);
    }
}
