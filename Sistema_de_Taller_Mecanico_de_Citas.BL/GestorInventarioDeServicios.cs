using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sistema_de_Taller_Mecanico_de_Citas.BL;
using Sistema_de_Taller_Mecanico_de_Citas.DA;
using Sistema_de_Taller_Mecanico_de_Citas.Model;
using System;
using System.Linq;
using System.Text;

namespace Sistema_de_Taller_Mecanico_de_Citas.BL
{ 
    public class GestorInventarioDeServicios : IGestorInventarioDeServicios
    {
        private readonly DA.TallerDbContext context;

        public GestorInventarioDeServicios(DA.TallerDbContext contexto)
        {
            context = contexto;
        }

        public List<Model.InventarioDeServicios> MuestreLaListaDeServicios()
        {
            var resultado = from c in context.InventarioDeServicios
                            select c;
            return resultado.ToList();
        }

        public Model.InventarioDeServicios ObtengaServicioPorId(int id)
        {
            List<Model.InventarioDeServicios> ListaDeServicios = MuestreLaListaDeServicios();
            Model.InventarioDeServicios Servicio = ListaDeServicios.FirstOrDefault(s => s.Id == id);
            return Servicio;
        }

        public void AgregueServicio(Model.InventarioDeServicios servicio)
        {
            if (servicio == null)
            {
                throw new ArgumentNullException(nameof(servicio), "El servicio no puede ser nulo.");
            }
            context.InventarioDeServicios.Add(servicio);
            context.SaveChanges();
        }

        public void EditeUnServicio(string nombre, string descripcion, int precio, int id)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del servicio no puede estar vacío.", nameof(nombre));
            }
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                throw new ArgumentException("La descripción del servicio no puede estar vacía.", nameof(descripcion));
            }
            if (precio <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(precio), "El precio debe ser mayor que cero.");
            }
            Model.InventarioDeServicios servicio = ObtengaServicioPorId(id);
            if (servicio == null)
            {
                throw new KeyNotFoundException($"No se encontró un servicio con el ID {id}.");
            }
            servicio.Nombre = nombre;
            servicio.Descripcion = descripcion;
            servicio.Precio = precio;
            context.SaveChanges();
        }

    }
}