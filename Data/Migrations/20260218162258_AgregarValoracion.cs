using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGDCP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarValoracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Valoraciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObraId = table.Column<int>(type: "int", nullable: false),
                    ValorDeterminado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaValoracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeritoId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Valoraciones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Valoraciones");
        }
    }
}
