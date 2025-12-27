using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerGame.Core.Migrations
{
    /// <inheritdoc />
    public partial class LeagueLocalTeamsAndRosterUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_teams_leagues_league_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "players");

            migrationBuilder.AlterColumn<Guid>(
                name: "league_id",
                table: "teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "league_id",
                table: "team_player",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_team_player_league_id_player_id",
                table: "team_player",
                columns: new[] { "league_id", "player_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_team_player_leagues_league_id",
                table: "team_player",
                column: "league_id",
                principalTable: "leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_teams_leagues_league_id",
                table: "teams",
                column: "league_id",
                principalTable: "leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_team_player_leagues_league_id",
                table: "team_player");

            migrationBuilder.DropForeignKey(
                name: "fk_teams_leagues_league_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "ix_team_player_league_id_player_id",
                table: "team_player");

            migrationBuilder.DropColumn(
                name: "league_id",
                table: "team_player");

            migrationBuilder.AlterColumn<Guid>(
                name: "league_id",
                table: "teams",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "team_id",
                table: "players",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_teams_leagues_league_id",
                table: "teams",
                column: "league_id",
                principalTable: "leagues",
                principalColumn: "id");
        }
    }
}
