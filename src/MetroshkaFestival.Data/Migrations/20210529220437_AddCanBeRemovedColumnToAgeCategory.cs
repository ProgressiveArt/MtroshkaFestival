using Microsoft.EntityFrameworkCore.Migrations;

namespace MetroshkaFestival.Data.Migrations
{
    public partial class AddCanBeRemovedColumnToAgeCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanBeRemoved",
                table: "AgeCategories",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanBeRemoved",
                table: "AgeCategories");
        }
    }
}
