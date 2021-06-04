using Microsoft.EntityFrameworkCore.Migrations;

namespace MetroshkaFestival.Data.Migrations
{
    public partial class Initial7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FirstTeamGoalsScore",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FirstTeamPenaltyGoalsScore",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatchFinalResult",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "SecondTeamGoalsScore",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondTeamPenaltyGoalsScore",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StageNumber",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstTeamGoalsScore",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "FirstTeamPenaltyGoalsScore",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "MatchFinalResult",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SecondTeamGoalsScore",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SecondTeamPenaltyGoalsScore",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "StageNumber",
                table: "Matches");
        }
    }
}
