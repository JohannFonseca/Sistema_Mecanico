using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using SistemaOrdenesReparacion.Model;

namespace SistemaOrdenesReparacion.DA
{
    public class TallerDbContext : DbContext
    {
        public TallerDbContext(DbContextOptions<TallerDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Mecanico> Mecanicos { get; set; }
        public DbSet<InventarioDeRepuesto> InventarioDeRepuestos { get; set; }
        public DbSet<InventarioDeServicios> InventarioDeServicios { get; set; }
        public DbSet<OrdenDeTrabajo> OrdenesDeTrabajos { get; set; }
        public DbSet<OrdenRepuesto> OrdenesDeTrabajoInventarioDeRepuestos { get; set; }
        public DbSet<OrdenServicio> OrdenesDeTrabajoInventarioDeServicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Usuario)
                .WithOne(u => u.Cliente)
                .HasForeignKey<Usuario>(u => u.Id_Cliente)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Mecanico>()
                .HasOne(m => m.Usuario)
                .WithOne(u => u.Mecanico)
                .HasForeignKey<Usuario>(u => u.Id_Mecanico)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<OrdenRepuesto>()
                .HasOne(or => or.OrdenDeTrabajo)
                .WithMany(ot => ot.Repuestos)
                .HasForeignKey(or => or.Id_OrdenesDeTrabajos);

            modelBuilder.Entity<OrdenRepuesto>()
                .HasOne(or => or.Repuesto)
                .WithMany(ir => ir.Ordenes)
                .HasForeignKey(or => or.Id_InventarioDeRepuestos);

            modelBuilder.Entity<OrdenServicio>()
                .HasOne(os => os.OrdenDeTrabajo)
                .WithMany(ot => ot.Servicios)
                .HasForeignKey(os => os.Id_OrdenesDeTrabajo);

            modelBuilder.Entity<OrdenServicio>()
                .HasOne(os => os.Servicio)
                .WithMany(isv => isv.Ordenes)
                .HasForeignKey(os => os.Id_InventarioDeServicios);

            modelBuilder.Entity<OrdenServicio>()
                .Property(os => os.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<OrdenRepuesto>()
    .Property(or => or.Total)
    .HasPrecision(18, 2);

            modelBuilder.Entity<OrdenServicio>()
                .Property(os => os.Total)
                .HasPrecision(18, 2);

        }
    }
}


