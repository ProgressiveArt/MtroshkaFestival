using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MetroshkaFestival.Data.Migrations
{
    public partial class UpdateAgeCategoriesAddBirthInterval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RangeOfBirthYears",
                table: "AgeCategories");

            migrationBuilder.AddColumn<DateTime>(
                name: "MaxBirthDate",
                table: "AgeCategories",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "MinBirthDate",
                table: "AgeCategories",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxBirthDate",
                table: "AgeCategories");

            migrationBuilder.DropColumn(
                name: "MinBirthDate",
                table: "AgeCategories");

            migrationBuilder.AddColumn<string>(
                name: "RangeOfBirthYears",
                table: "AgeCategories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
