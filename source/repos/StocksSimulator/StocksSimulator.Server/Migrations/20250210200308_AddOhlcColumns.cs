using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StocksSimulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddOhlcColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "StockPrices",
                newName: "Open");

            migrationBuilder.AddColumn<decimal>(
                name: "Close",
                table: "StockPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "High",
                table: "StockPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Low",
                table: "StockPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "Volume",
                table: "StockPrices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Close",
                table: "StockPrices");

            migrationBuilder.DropColumn(
                name: "High",
                table: "StockPrices");

            migrationBuilder.DropColumn(
                name: "Low",
                table: "StockPrices");

            migrationBuilder.DropColumn(
                name: "Volume",
                table: "StockPrices");

            migrationBuilder.RenameColumn(
                name: "Open",
                table: "StockPrices",
                newName: "Price");
        }
    }
}
