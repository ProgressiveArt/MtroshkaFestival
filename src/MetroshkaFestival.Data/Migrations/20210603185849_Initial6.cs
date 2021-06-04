using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MetroshkaFestival.Data.Migrations
{
    public partial class Initial6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AgeCategoryTournamentId = table.Column<int>(type: "integer", nullable: false),
                    AgeCategoryAgeGroup = table.Column<int>(type: "integer", nullable: false),
                    MatchDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FieldNumber = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    FirstTeamId = table.Column<int>(type: "integer", nullable: false),
                    SecondTeamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_AgeCategories_AgeCategoryTournamentId_AgeCategoryAg~",
                        columns: x => new { x.AgeCategoryTournamentId, x.AgeCategoryAgeGroup },
                        principalTable: "AgeCategories",
                        principalColumns: new[] { "TournamentId", "AgeGroup" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_FirstTeamId",
                        column: x => x.FirstTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_SecondTeamId",
                        column: x => x.SecondTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_AgeCategoryTournamentId_AgeCategoryAgeGroup",
                table: "Matches",
                columns: new[] { "AgeCategoryTournamentId", "AgeCategoryAgeGroup" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_FirstTeamId",
                table: "Matches",
                column: "FirstTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SecondTeamId",
                table: "Matches",
                column: "SecondTeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AgeCategoryAgeGroup = table.Column<int>(type: "integer", nullable: false),
                    AgeCategoryTournamentId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_AgeCategories_AgeCategoryTournamentId_AgeCategoryAge~",
                        columns: x => new { x.AgeCategoryTournamentId, x.AgeCategoryAgeGroup },
                        principalTable: "AgeCategories",
                        principalColumns: new[] { "TournamentId", "AgeGroup" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_AgeCategoryTournamentId_AgeCategoryAgeGroup",
                table: "Groups",
                columns: new[] { "AgeCategoryTournamentId", "AgeCategoryAgeGroup" });
        }
    }
}
