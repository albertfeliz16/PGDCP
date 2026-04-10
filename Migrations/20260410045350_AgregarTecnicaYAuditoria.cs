using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PGDCP.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTecnicaYAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Ubicaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "TecnicaId",
                table: "Obras",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Materiales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Estilos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Epocas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "EstadoConservacionId",
                table: "Conservaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Categorias",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "AuditoriaLogin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exitoso = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaLogin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosConservacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosConservacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogCambios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entidad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistroId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorAnterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorNuevo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogCambios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tecnicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Obras_TecnicaId",
                table: "Obras",
                column: "TecnicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Conservaciones_EstadoConservacionId",
                table: "Conservaciones",
                column: "EstadoConservacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conservaciones_EstadosConservacion_EstadoConservacionId",
                table: "Conservaciones",
                column: "EstadoConservacionId",
                principalTable: "EstadosConservacion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Obras_Tecnicas_TecnicaId",
                table: "Obras",
                column: "TecnicaId",
                principalTable: "Tecnicas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conservaciones_EstadosConservacion_EstadoConservacionId",
                table: "Conservaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Obras_Tecnicas_TecnicaId",
                table: "Obras");

            migrationBuilder.DropTable(
                name: "AuditoriaLogin");

            migrationBuilder.DropTable(
                name: "EstadosConservacion");

            migrationBuilder.DropTable(
                name: "LogCambios");

            migrationBuilder.DropTable(
                name: "Tecnicas");

            migrationBuilder.DropIndex(
                name: "IX_Obras_TecnicaId",
                table: "Obras");

            migrationBuilder.DropIndex(
                name: "IX_Conservaciones_EstadoConservacionId",
                table: "Conservaciones");

            migrationBuilder.DropColumn(
                name: "TecnicaId",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "EstadoConservacionId",
                table: "Conservaciones");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Ubicaciones",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Materiales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Estilos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Epocas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Categorias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
