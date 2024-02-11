using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor_2024.Server.Migrations
{
    /// <inheritdoc />
    public partial class _7_add_table_listaslibros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListasLibros",
                columns: table => new
                {
                    IdLista = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreLista = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LibrosLista = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasLibros", x => x.IdLista);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListasLibros");
        }
    }
}
