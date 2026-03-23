using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSAapp.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloArchivisticoYChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Oficios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FolioInterno = table.Column<string>(type: "TEXT", nullable: false),
                    FolioOriginal = table.Column<string>(type: "TEXT", nullable: true),
                    Remitente = table.Column<string>(type: "TEXT", nullable: false),
                    Asunto = table.Column<string>(type: "TEXT", nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaLimite = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UsuarioAsignado = table.Column<string>(type: "TEXT", nullable: false),
                    Estatus = table.Column<string>(type: "TEXT", nullable: false),
                    RutaArchivoRed = table.Column<string>(type: "TEXT", nullable: true),
                    CapturadoPor = table.Column<string>(type: "TEXT", nullable: false),
                    BloqueadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    BloqueadoDesde = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UbicacionLefort = table.Column<string>(type: "TEXT", nullable: true),
                    UbicacionCaja = table.Column<string>(type: "TEXT", nullable: true),
                    UbicacionEstante = table.Column<string>(type: "TEXT", nullable: true),
                    CadidoFondo = table.Column<string>(type: "TEXT", nullable: true),
                    CadidoSeccion = table.Column<string>(type: "TEXT", nullable: true),
                    CadidoSerie = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oficios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TareasInstitucionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    RutaArchivoAdjunto = table.Column<string>(type: "TEXT", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstaCompletada = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreadoPor = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareasInstitucionales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComentariosOficio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OficioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Usuario = table.Column<string>(type: "TEXT", nullable: false),
                    Mensaje = table.Column<string>(type: "TEXT", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosOficio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComentariosOficio_Oficios_OficioId",
                        column: x => x.OficioId,
                        principalTable: "Oficios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosOficio_OficioId",
                table: "ComentariosOficio",
                column: "OficioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComentariosOficio");

            migrationBuilder.DropTable(
                name: "TareasInstitucionales");

            migrationBuilder.DropTable(
                name: "Oficios");
        }
    }
}
