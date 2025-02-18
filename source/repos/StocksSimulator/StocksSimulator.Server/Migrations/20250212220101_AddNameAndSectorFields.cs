using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StocksSimulator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNameAndSectorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Securities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "Securities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Securities");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "Securities");
        }
    }
}
