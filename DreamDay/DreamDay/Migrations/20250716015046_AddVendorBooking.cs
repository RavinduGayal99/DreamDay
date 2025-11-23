using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamDay.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "VendorBookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "VendorBookings");
        }
    }
}
