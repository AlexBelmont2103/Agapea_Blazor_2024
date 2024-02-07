using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor_2024.Server.Migrations
{
    /// <inheritdoc />
    public partial class add_tabla_paypal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdCliente",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdCliente",
                table: "Direcciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PedidosPayPal",
                columns: table => new
                {
                    IdPedido = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosPayPal", x => x.IdPedido);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidosPayPal");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "Direcciones");
        }
    }
}
