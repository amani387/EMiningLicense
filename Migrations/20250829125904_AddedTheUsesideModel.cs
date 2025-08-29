using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMiningLicense.Migrations
{
    /// <inheritdoc />
    public partial class AddedTheUsesideModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "LicenseApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "LicenseApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "LicenseApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "LicenseApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "LicenseApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "LicenseApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "LicenseApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsRenewal",
                table: "LicenseApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "LicenseApplications",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "LicenseApplications",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "MiningType",
                table: "LicenseApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "LicenseApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "IsRenewal",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "MiningType",
                table: "LicenseApplications");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "LicenseApplications");
        }
    }
}
