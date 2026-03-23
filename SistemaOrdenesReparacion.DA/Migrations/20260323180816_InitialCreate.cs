using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaOrdenesReparacion.DA.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventarioDeRepuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioDeRepuestos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventarioDeServicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioDeServicios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mecanicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Identificacion = table.Column<int>(type: "int", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mecanicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesDeTrabajos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Cliente = table.Column<int>(type: "int", nullable: false),
                    FechaDeRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAproximadaDeFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Id_Mecanico = table.Column<int>(type: "int", nullable: true),
                    TipoDeVehiculo = table.Column<int>(type: "int", nullable: false),
                    TipoDeCombustionInterna = table.Column<int>(type: "int", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AñoDeFabricacion = table.Column<int>(type: "int", nullable: false),
                    DescripcionDelProblema = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotivoDeCancelacion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeTrabajos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajos_Clientes_Id_Cliente",
                        column: x => x.Id_Cliente,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajos_Mecanicos_Id_Mecanico",
                        column: x => x.Id_Mecanico,
                        principalTable: "Mecanicos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    BloqueadoHasta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaUltimoCambioClave = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IntentosFallidos = table.Column<int>(type: "int", nullable: false),
                    Id_Cliente = table.Column<int>(type: "int", nullable: true),
                    Id_Mecanico = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Clientes_Id_Cliente",
                        column: x => x.Id_Cliente,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Usuarios_Mecanicos_Id_Mecanico",
                        column: x => x.Id_Mecanico,
                        principalTable: "Mecanicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesDeTrabajoInventarioDeRepuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_OrdenesDeTrabajos = table.Column<int>(type: "int", nullable: false),
                    Id_InventarioDeRepuestos = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeTrabajoInventarioDeRepuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajoInventarioDeRepuestos_InventarioDeRepuestos_Id_InventarioDeRepuestos",
                        column: x => x.Id_InventarioDeRepuestos,
                        principalTable: "InventarioDeRepuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajoInventarioDeRepuestos_OrdenesDeTrabajos_Id_OrdenesDeTrabajos",
                        column: x => x.Id_OrdenesDeTrabajos,
                        principalTable: "OrdenesDeTrabajos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesDeTrabajoInventarioDeServicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_OrdenesDeTrabajo = table.Column<int>(type: "int", nullable: false),
                    Id_InventarioDeServicios = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeTrabajoInventarioDeServicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajoInventarioDeServicios_InventarioDeServicios_Id_InventarioDeServicios",
                        column: x => x.Id_InventarioDeServicios,
                        principalTable: "InventarioDeServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajoInventarioDeServicios_OrdenesDeTrabajos_Id_OrdenesDeTrabajo",
                        column: x => x.Id_OrdenesDeTrabajo,
                        principalTable: "OrdenesDeTrabajos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeTrabajoInventarioDeRepuestos_Id_InventarioDeRepuestos",
                table: "OrdenesDeTrabajoInventarioDeRepuestos",
                column: "Id_InventarioDeRepuestos");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeTrabajoInventarioDeRepuestos_Id_OrdenesDeTrabajos",
                table: "OrdenesDeTrabajoInventarioDeRepuestos",
                column: "Id_OrdenesDeTrabajos");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeTrabajoInventarioDeServicios_Id_InventarioDeServicios",
                table: "OrdenesDeTrabajoInventarioDeServicios",
                column: "Id_InventarioDeServicios");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeTrabajoInventarioDeServicios_Id_OrdenesDeTrabajo",
                table: "OrdenesDeTrabajoInventarioDeServicios",
                column: "Id_OrdenesDeTrabajo");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeTrabajos_Id_Cliente",
                table: "OrdenesDeTrabajos",
                column: "Id_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeTrabajos_Id_Mecanico",
                table: "OrdenesDeTrabajos",
                column: "Id_Mecanico");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Id_Cliente",
                table: "Usuarios",
                column: "Id_Cliente",
                unique: true,
                filter: "[Id_Cliente] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Id_Mecanico",
                table: "Usuarios",
                column: "Id_Mecanico",
                unique: true,
                filter: "[Id_Mecanico] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenesDeTrabajoInventarioDeRepuestos");

            migrationBuilder.DropTable(
                name: "OrdenesDeTrabajoInventarioDeServicios");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "InventarioDeRepuestos");

            migrationBuilder.DropTable(
                name: "InventarioDeServicios");

            migrationBuilder.DropTable(
                name: "OrdenesDeTrabajos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Mecanicos");
        }
    }
}
