using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyGamesWeb.Migrations
{
    /// <inheritdoc />
    public partial class ty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNormalized = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopInventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopInventories_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopSales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CreateDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopSales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopSales_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ShopSales_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopUsers",
                columns: table => new
                {
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsManager = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopUsers", x => new { x.ShopId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ShopUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopUsers_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopSaleLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopSaleId = table.Column<int>(type: "int", nullable: false),
                    ShopInventoryId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopSaleLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopSaleLines_ShopInventories_ShopInventoryId",
                        column: x => x.ShopInventoryId,
                        principalTable: "ShopInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopSaleLines_ShopSales_ShopSaleId",
                        column: x => x.ShopSaleId,
                        principalTable: "ShopSales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNormalized",
                table: "Customers",
                column: "PhoneNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopInventories_ProductId",
                table: "ShopInventories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopInventories_ShopId_ProductId",
                table: "ShopInventories",
                columns: new[] { "ShopId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopSaleLines_ShopInventoryId",
                table: "ShopSaleLines",
                column: "ShopInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopSaleLines_ShopSaleId",
                table: "ShopSaleLines",
                column: "ShopSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopSales_CustomerId",
                table: "ShopSales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopSales_ShopId",
                table: "ShopSales",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopUsers_UserId",
                table: "ShopUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopSaleLines");

            migrationBuilder.DropTable(
                name: "ShopUsers");

            migrationBuilder.DropTable(
                name: "ShopInventories");

            migrationBuilder.DropTable(
                name: "ShopSales");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Shops");
        }
    }
}
