using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGDCP.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaObra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Obras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Epoca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estilo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaAdquisicion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obras", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Obras");
        }
    }
}
