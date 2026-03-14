using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGDCP.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelacionObraUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Obras",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Obras");
        }
    }
}
