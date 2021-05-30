using Microsoft.EntityFrameworkCore.Migrations;

namespace MetroshkaFestival.Data.Migrations
{
    public partial class initial1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "NumberInTeam",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberInTeam",
                table: "Players");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Players",
                type: "text",
                nullable: true);
        }
    }
}
