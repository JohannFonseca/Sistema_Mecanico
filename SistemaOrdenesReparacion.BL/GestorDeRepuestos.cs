using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaOrdenesReparacion.BL
{
    public class GestorDeRepuestos : IGestorDeRepuestos
    {
        private readonly DA.TallerDbContext _context;

        public GestorDeRepuestos(DA.TallerDbContext context)
        {
            _context = context;
        }

        public List<Model.InventarioDeRepuesto> ObtenerRepuestos()
        {
            var resultado = from c in _context.InventarioDeRepuestos
                           select c;
            return resultado.ToList();
        }

        public Model.InventarioDeRepuesto ObtenerRepuestoPorId(int id)
        {
            List<Model.InventarioDeRepuesto> ListaDeRepuestos = ObtenerRepuestos();
            Model.InventarioDeRepuesto Repuesto = ListaDeRepuestos.FirstOrDefault(r => r.Id == id);
            return Repuesto;
        }

        public void AgregarRepuesto(Model.InventarioDeRepuesto repuesto)
        {
            if (repuesto == null)
            {
                throw new ArgumentNullException(nameof(repuesto), "El repuesto no puede ser nulo.");
            }
            _context.InventarioDeRepuestos.Add(repuesto);
            _context.SaveChanges();
        }

        public void EditeUnRepuesto(string nombre, string descripcion, int precio, int id)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del repuesto no puede estar vacío.", nameof(nombre));
            }
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                throw new ArgumentException("La descripción del repuesto no puede estar vacía.", nameof(descripcion));
            }
            if (precio <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(precio), "El precio debe ser mayor que cero.");
            }
            Model.InventarioDeRepuesto repuesto = ObtenerRepuestoPorId(id);
            if (repuesto == null)
            {
                throw new KeyNotFoundException($"No se encontró un repuesto con el ID {id}.");
            }
            repuesto.Nombre = nombre;
            repuesto.Descripcion = descripcion;
            repuesto.Precio = precio;
            _context.SaveChanges();
        }


    }
}
