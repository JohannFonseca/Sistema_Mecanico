using Microsoft.EntityFrameworkCore;
using SistemaOrdenesReparacion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenesReparacion.BL
{
    public class GestorDeOrdenesEnProceso : IGestorDeOrdenesEnProceso
    {
        private readonly DA.TallerDbContext context;

        public GestorDeOrdenesEnProceso(DA.TallerDbContext contexto)
        {
            context = contexto;
        }

        public List<OrdenDeTrabajo> ObtengaLasOrdenesEnProceso()
        {
            return context.OrdenesDeTrabajos
                .Include(o => o.Cliente)
                .Include(o => o.Mecanico)
                .Where(o => o.Estado == Model.EstadoOrden.EnProceso)
                .ToList();
        }


        public Model.OrdenDeTrabajo ObtengaOrdenPorId(int id)
        {
            List<Model.OrdenDeTrabajo> listaDeOrdenes = ObtengaLasOrdenesEnProceso();
            Model.OrdenDeTrabajo orden = listaDeOrdenes.FirstOrDefault(o => o.Id == id);
            return orden;
        }

        public void AgregarRepuestoAOrden(int idOrden, int idRepuesto, int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

            var repuesto = context.InventarioDeRepuestos.FirstOrDefault(r => r.Id == idRepuesto);
            if (repuesto == null)
                throw new InvalidOperationException("El repuesto no existe.");

            var orden = context.OrdenesDeTrabajos.FirstOrDefault(o => o.Id == idOrden);
            if (orden == null)
                throw new InvalidOperationException("La orden no existe.");

            // Verificar si el repuesto ya está asignado a la orden
            var existente = context.OrdenesDeTrabajoInventarioDeRepuestos
                .FirstOrDefault(or => or.Id_OrdenesDeTrabajos == idOrden && or.Id_InventarioDeRepuestos == idRepuesto);

            if (existente != null)
            {
                // Si ya existe, se suma la cantidad y se recalcula el total
                existente.Cantidad += cantidad;
                existente.Total = existente.Cantidad * repuesto.Precio;
            }
            else
            {
                // Si no existe, se crea uno nuevo
                var ordenRepuesto = new OrdenRepuesto
                {
                    Id_OrdenesDeTrabajos = idOrden,
                    Id_InventarioDeRepuestos = idRepuesto,
                    Cantidad = cantidad,
                    Total = repuesto.Precio * cantidad
                };

                context.OrdenesDeTrabajoInventarioDeRepuestos.Add(ordenRepuesto);
            }

            context.SaveChanges();
        }
        public List<OrdenRepuesto> ObtenerRepuestosAsignadosAOrden(int idOrden)
        {
            return context.OrdenesDeTrabajoInventarioDeRepuestos
                .Where(or => or.Id_OrdenesDeTrabajos == idOrden)
                .Include(or => or.Repuesto)
                .ToList();
        }

        public void AgregarServicioAOrden(int idOrden, int idServicio, int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

            var servicio = context.InventarioDeServicios.FirstOrDefault(s => s.Id == idServicio);
            if (servicio == null)
                throw new InvalidOperationException("El servicio no existe.");

            var orden = context.OrdenesDeTrabajos.FirstOrDefault(o => o.Id == idOrden);
            if (orden == null)
                throw new InvalidOperationException("La orden no existe.");

            // Validación para actualizar si ya existe
            var servicioExistente = context.OrdenesDeTrabajoInventarioDeServicios
                .FirstOrDefault(os => os.Id_OrdenesDeTrabajo == idOrden && os.Id_InventarioDeServicios == idServicio);

            if (servicioExistente != null)
            {
                servicioExistente.Cantidad += cantidad;
                servicioExistente.Total = servicioExistente.Cantidad * servicio.Precio;
            }
            else
            {
                var ordenServicio = new OrdenServicio
                {
                    Id_OrdenesDeTrabajo = idOrden,
                    Id_InventarioDeServicios = idServicio,
                    Cantidad = cantidad,
                    Total = servicio.Precio * cantidad
                };

                context.OrdenesDeTrabajoInventarioDeServicios.Add(ordenServicio);
            }

            context.SaveChanges();
        }

        public List<OrdenServicio> ObtenerServiciosAsignadosAOrden(int idOrden)
        {
            return context.OrdenesDeTrabajoInventarioDeServicios
                .Where(os => os.Id_OrdenesDeTrabajo == idOrden)
                .Include(os => os.Servicio) 
                .ToList();
        }

        public ResumenTotal ObtenerTotalesDeOrden(int idOrden)
        {
            var totalRepuestos = context.OrdenesDeTrabajoInventarioDeRepuestos
                .Where(or => or.Id_OrdenesDeTrabajos == idOrden)
                .Sum(or => (decimal?)or.Total) ?? 0;

            var totalServicios = context.OrdenesDeTrabajoInventarioDeServicios
                .Where(os => os.Id_OrdenesDeTrabajo == idOrden)
                .Sum(os => (decimal?)os.Total) ?? 0;

            return new ResumenTotal
            {
                TotalRepuestos = totalRepuestos,
                TotalServicios = totalServicios
            };
        }
    }
}
