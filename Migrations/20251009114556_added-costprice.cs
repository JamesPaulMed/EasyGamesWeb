using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyGamesWeb.Migrations
{
    /// <inheritdoc />
    public partial class addedcostprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCostAtSale",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPriceAtSale",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UnitCostAtSale",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "UnitPriceAtSale",
                table: "OrderDetails");
        }
    }
}
