using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor_2024.Server.Migrations
{
    /// <inheritdoc />
    public partial class _5_add_tabla_opiniones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Opiniones",
                columns: table => new
                {
                    IdOpinion = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isbn13 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valoracion = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsModerada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opiniones", x => x.IdOpinion);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Opiniones");
        }
    }
}
