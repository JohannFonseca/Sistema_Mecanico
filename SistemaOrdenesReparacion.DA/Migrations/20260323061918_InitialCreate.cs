using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventarioDeRepuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellidos = table.Column<string>(type: "text", nullable: false),
                    Identificacion = table.Column<int>(type: "integer", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mecanicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesDeTrabajos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Cliente = table.Column<int>(type: "integer", nullable: false),
                    FechaDeRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaAproximadaDeFinalizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Id_Mecanico = table.Column<int>(type: "integer", nullable: true),
                    TipoDeVehiculo = table.Column<int>(type: "integer", nullable: false),
                    TipoDeCombustionInterna = table.Column<int>(type: "integer", nullable: false),
                    Marca = table.Column<string>(type: "text", nullable: false),
                    Modelo = table.Column<string>(type: "text", nullable: false),
                    AñoDeFabricacion = table.Column<int>(type: "integer", nullable: false),
                    DescripcionDelProblema = table.Column<string>(type: "text", nullable: false),
                    MotivoDeCancelacion = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Clave = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    BloqueadoHasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaUltimoCambioClave = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IntentosFallidos = table.Column<int>(type: "integer", nullable: false),
                    Id_Cliente = table.Column<int>(type: "integer", nullable: true),
                    Id_Mecanico = table.Column<int>(type: "integer", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_OrdenesDeTrabajos = table.Column<int>(type: "integer", nullable: false),
                    Id_InventarioDeRepuestos = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeTrabajoInventarioDeRepuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajoInventarioDeRepuestos_InventarioDeRepuestos~",
                        column: x => x.Id_InventarioDeRepuestos,
                        principalTable: "InventarioDeRepuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajoInventarioDeRepuestos_OrdenesDeTrabajos_Id_~",
                        column: x => x.Id_OrdenesDeTrabajos,
                        principalTable: "OrdenesDeTrabajos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesDeTrabajoInventarioDeServicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_OrdenesDeTrabajo = table.Column<int>(type: "integer", nullable: false),
                    Id_InventarioDeServicios = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeTrabajoInventarioDeServicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajoInventarioDeServicios_InventarioDeServicios~",
                        column: x => x.Id_InventarioDeServicios,
                        principalTable: "InventarioDeServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesDeTrabajoInventarioDeServicios_OrdenesDeTrabajos_Id_~",
                        column: x => x.Id_OrdenesDeTrabajo,
                        principalTable: "OrdenesDeTrabajos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeTrabajoInventarioDeRepuestos_Id_InventarioDeRepues~",
                table: "OrdenesDeTrabajoInventarioDeRepuestos",
                column: "Id_InventarioDeRepuestos");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeTrabajoInventarioDeRepuestos_Id_OrdenesDeTrabajos",
                table: "OrdenesDeTrabajoInventarioDeRepuestos",
                column: "Id_OrdenesDeTrabajos");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeTrabajoInventarioDeServicios_Id_InventarioDeServic~",
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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Id_Mecanico",
                table: "Usuarios",
                column: "Id_Mecanico",
                unique: true);
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
