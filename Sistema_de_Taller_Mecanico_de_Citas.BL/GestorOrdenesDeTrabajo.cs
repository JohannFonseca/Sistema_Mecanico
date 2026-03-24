using System;
using System.Collections.Generic;
using System.Linq;
using Sistema_de_Taller_Mecanico_de_Citas.DA;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using Microsoft.EntityFrameworkCore;

namespace Sistema_de_Taller_Mecanico_de_Citas.BL
{
    public class GestorDeOrdenesDeTrabajo : IGestorOrdenesDeTrabajo
    {
        private readonly TallerDbContext elContexto;

        public GestorDeOrdenesDeTrabajo(TallerDbContext contexto)
        {
            elContexto = contexto;
        }

        public List<OrdenDeTrabajo> MuestreLaListaDeOrdenes(string filtroNombreCliente)
        {
            string filtro = filtroNombreCliente ?? string.Empty;

            var listaOrdenes = elContexto.OrdenesDeTrabajos
     .Include(o => o.Cliente)
     .Where(o => o.Estado != EstadoOrden.Cancelado &&
                (string.IsNullOrEmpty(filtro) || (o.Cliente != null && o.Cliente.Nombre.Contains(filtro))))
     .ToList();



            foreach (var orden in listaOrdenes)
            {
                if (orden.Cliente == null)
                    orden.Cliente = new Cliente();

                if (orden.Mecanico == null)
                    orden.Mecanico = new Mecanico();

                if (orden.Repuestos == null)
                    orden.Repuestos = new List<OrdenRepuesto>();

                if (orden.Servicios == null)
                    orden.Servicios = new List<OrdenServicio>();
            }

            return listaOrdenes;
        }
        public void AgregueOrdenDeTrabajo(OrdenDeTrabajo orden)
        {
            if (orden == null)
                throw new ArgumentNullException(nameof(orden));

            if (orden.Id_Cliente <= 0)
                throw new ArgumentException("Debe seleccionar un cliente.");

            if (orden.FechaAproximadaDeFinalizacion == default)
                throw new ArgumentException("La fecha aproximada de finalización es requerida.");

            if (string.IsNullOrWhiteSpace(orden.Marca))
                throw new ArgumentException("La marca es requerida.");

            if (string.IsNullOrWhiteSpace(orden.Modelo))
                throw new ArgumentException("El modelo es requerido.");

            if (orden.AñoDeFabricacion <= 0)
                throw new ArgumentException("El año de fabricación es requerido.");

            if (string.IsNullOrWhiteSpace(orden.DescripcionDelProblema))
                throw new ArgumentException("La descripción del problema es requerida.");

            orden.Estado = EstadoOrden.Registrado;

            orden.FechaDeRegistro = DateTime.Now;

            elContexto.OrdenesDeTrabajos.Add(orden);
            elContexto.SaveChanges();
        }


        public void EditeOrdenDeTrabajo(OrdenDeTrabajo orden)
        {
            if (orden == null)
                throw new ArgumentNullException(nameof(orden));

            var ordenExistente = elContexto.OrdenesDeTrabajos.Find(orden.Id);

            if (ordenExistente == null)
                throw new ArgumentException("La orden no existe.");

            if (orden.Id_Cliente <= 0)
                throw new ArgumentException("Debe seleccionar un cliente.");

            if (orden.FechaAproximadaDeFinalizacion == default)
                throw new ArgumentException("La fecha aproximada de finalización es requerida.");

            if (string.IsNullOrWhiteSpace(orden.Marca))
                throw new ArgumentException("La marca es requerida.");

            if (string.IsNullOrWhiteSpace(orden.Modelo))
                throw new ArgumentException("El modelo es requerido.");

            if (orden.AñoDeFabricacion == null)
                throw new ArgumentException("El año de fabricación es requerido.");

            if (string.IsNullOrWhiteSpace(orden.DescripcionDelProblema))
                throw new ArgumentException("La descripción del problema es requerida.");

            ordenExistente.Id_Cliente = orden.Id_Cliente;
            ordenExistente.FechaAproximadaDeFinalizacion = orden.FechaAproximadaDeFinalizacion;
            ordenExistente.TipoDeVehiculo = orden.TipoDeVehiculo;
            ordenExistente.TipoDeCombustionInterna = orden.TipoDeCombustionInterna;
            ordenExistente.Marca = orden.Marca;
            ordenExistente.Modelo = orden.Modelo;
            ordenExistente.AñoDeFabricacion = orden.AñoDeFabricacion;
            ordenExistente.DescripcionDelProblema = orden.DescripcionDelProblema;
            ordenExistente.Estado = orden.Estado;
            ordenExistente.MontoTotal = orden.MontoTotal;
            ordenExistente.MetodoPago = orden.MetodoPago;
            ordenExistente.Pagado = orden.Pagado;
            ordenExistente.FechaDeEntregaReal = orden.FechaDeEntregaReal;

            elContexto.SaveChanges();
        }

        public OrdenDeTrabajo ObtenerOrdenPorId(int id)
        {
            return elContexto.OrdenesDeTrabajos
                .Include(o => o.Cliente)
                .Include(o => o.Mecanico)
                .Include(o => o.Servicios).ThenInclude(s => s.Servicio)
                .Include(o => o.Repuestos).ThenInclude(r => r.Repuesto)
                .FirstOrDefault(o => o.Id == id);
        }

        public void CanceleOrdenDeTrabajo(int idOrden, string motivoCancelacion)
        {
            if (string.IsNullOrWhiteSpace(motivoCancelacion))
                throw new ArgumentException("El motivo de cancelación es requerido.");

            var orden = elContexto.OrdenesDeTrabajos.Find(idOrden);

            if (orden == null)
                throw new ArgumentException("La orden no existe.");

            orden.MotivoDeCancelacion = motivoCancelacion;
            orden.Estado = EstadoOrden.Cancelado;

            elContexto.SaveChanges();
        }

        public void InicieOrdenDeTrabajo(int idOrden, int idMecanico)
        {
            var orden = elContexto.OrdenesDeTrabajos.Find(idOrden);

            if (orden == null)
                throw new ArgumentException("La orden no existe.");

            var mecanico = elContexto.Mecanicos.Find(idMecanico);

            if (mecanico == null)
                throw new ArgumentException("El mecánico no existe.");

            orden.Id_Mecanico = idMecanico;
            orden.Estado = EstadoOrden.EnProceso;

            elContexto.SaveChanges();
        }
    }
}
