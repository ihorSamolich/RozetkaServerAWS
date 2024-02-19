using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebRozetka.Migrations
{
    /// <inheritdoc />
    public partial class Addlocationtowarehouseentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Latitude",
                table: "Warehouses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Longitude",
                table: "Warehouses",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Warehouses");
        }
    }
}
