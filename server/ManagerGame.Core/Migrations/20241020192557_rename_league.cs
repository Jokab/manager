using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerGame.Core.Migrations
{
    /// <inheritdoc />
    public partial class rename_league : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drafts_league_league_id",
                table: "drafts");

            migrationBuilder.DropForeignKey(
                name: "fk_teams_league_league_id",
                table: "teams");

            migrationBuilder.DropPrimaryKey(
                name: "pk_league",
                table: "league");

            migrationBuilder.RenameTable(
                name: "league",
                newName: "leagues");

            migrationBuilder.RenameColumn(
                name: "DraftOrderPrevious",
                table: "drafts",
                newName: "draftOrderPrevious");

            migrationBuilder.RenameColumn(
                name: "DraftOrderCurrent",
                table: "drafts",
                newName: "draftOrderCurrent");

            migrationBuilder.AddPrimaryKey(
                name: "pk_leagues",
                table: "leagues",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_drafts_leagues_league_id",
                table: "drafts",
                column: "league_id",
                principalTable: "leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_teams_leagues_league_id",
                table: "teams",
                column: "league_id",
                principalTable: "leagues",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drafts_leagues_league_id",
                table: "drafts");

            migrationBuilder.DropForeignKey(
                name: "fk_teams_leagues_league_id",
                table: "teams");

            migrationBuilder.DropPrimaryKey(
                name: "pk_leagues",
                table: "leagues");

            migrationBuilder.RenameTable(
                name: "leagues",
                newName: "league");

            migrationBuilder.RenameColumn(
                name: "draftOrderPrevious",
                table: "drafts",
                newName: "DraftOrderPrevious");

            migrationBuilder.RenameColumn(
                name: "draftOrderCurrent",
                table: "drafts",
                newName: "DraftOrderCurrent");

            migrationBuilder.AddPrimaryKey(
                name: "pk_league",
                table: "league",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_drafts_league_league_id",
                table: "drafts",
                column: "league_id",
                principalTable: "league",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_teams_league_league_id",
                table: "teams",
                column: "league_id",
                principalTable: "league",
                principalColumn: "id");
        }
    }
}
