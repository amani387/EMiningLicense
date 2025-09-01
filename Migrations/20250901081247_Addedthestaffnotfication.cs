using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMiningLicense.Migrations
{
    /// <inheritdoc />
    public partial class Addedthestaffnotfication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedStaffId",
                table: "LicenseApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedStaffId",
                table: "LicenseApplications");
        }
    }
}
