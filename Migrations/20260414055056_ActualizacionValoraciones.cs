using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGDCP.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionValoraciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstadoAutenticidad",
                table: "Valoraciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FactoresAjuste",
                table: "Valoraciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetodoValoracion",
                table: "Valoraciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoAutenticidad",
                table: "Valoraciones");

            migrationBuilder.DropColumn(
                name: "FactoresAjuste",
                table: "Valoraciones");

            migrationBuilder.DropColumn(
                name: "MetodoValoracion",
                table: "Valoraciones");
        }
    }
}
