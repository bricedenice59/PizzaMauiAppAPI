using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaMauiApp.API.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PizzaProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MainImageUrl = table.Column<string>(type: "text", nullable: false),
                    PriceWithExcludedVAT = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Ingredients = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PizzaProductImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PizzaProductImages_PizzaProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "PizzaProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PizzaProductImages_ProductId",
                table: "PizzaProductImages",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PizzaProductImages");

            migrationBuilder.DropTable(
                name: "PizzaProducts");
        }
    }
}
