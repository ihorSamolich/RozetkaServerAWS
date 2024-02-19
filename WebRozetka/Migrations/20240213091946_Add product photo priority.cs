using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebRozetka.Migrations
{
    /// <inheritdoc />
    public partial class Addproductphotopriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Photos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Photos");
        }
    }
}
