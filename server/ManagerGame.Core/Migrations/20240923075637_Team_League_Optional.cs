using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerGame.Core.Migrations
{
    /// <inheritdoc />
    public partial class Team_League_Optional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_teams_league_league_id",
                table: "teams");

            migrationBuilder.AlterColumn<Guid>(
                name: "league_id",
                table: "teams",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_teams_league_league_id",
                table: "teams",
                column: "league_id",
                principalTable: "league",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_teams_league_league_id",
                table: "teams");

            migrationBuilder.AlterColumn<Guid>(
                name: "league_id",
                table: "teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_teams_league_league_id",
                table: "teams",
                column: "league_id",
                principalTable: "league",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
