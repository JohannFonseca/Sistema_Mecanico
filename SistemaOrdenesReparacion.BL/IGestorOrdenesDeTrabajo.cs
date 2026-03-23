using System;
using System.Collections.Generic;
using SistemaOrdenesReparacion.Model;



namespace SistemaOrdenesReparacion.BL
{
    public interface IGestorOrdenesDeTrabajo
    {
        List<OrdenDeTrabajo> MuestreLaListaDeOrdenes(string filtroNombreCliente);

        void AgregueOrdenDeTrabajo(OrdenDeTrabajo orden);

        void EditeOrdenDeTrabajo(OrdenDeTrabajo orden);

        void CanceleOrdenDeTrabajo(int idOrden, string motivoCancelacion);

        void InicieOrdenDeTrabajo(int idOrden, int idMecanico);
    }
}
