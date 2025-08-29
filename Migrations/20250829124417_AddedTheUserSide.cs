using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMiningLicense.Migrations
{
    /// <inheritdoc />
    public partial class AddedTheUserSide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicenseType",
                table: "LicenseApplications",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicantId",
                table: "LicenseApplications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseApplications_ApplicantId",
                table: "LicenseApplications",
                column: "ApplicantId");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseApplications_AspNetUsers_ApplicantId",
                table: "LicenseApplications",
                column: "ApplicantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseApplications_AspNetUsers_ApplicantId",
                table: "LicenseApplications");

            migrationBuilder.DropIndex(
                name: "IX_LicenseApplications_ApplicantId",
                table: "LicenseApplications");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "LicenseApplications",
                newName: "LicenseType");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicantId",
                table: "LicenseApplications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
