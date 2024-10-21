using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerGame.Core.Migrations
{
    /// <inheritdoc />
    public partial class league_draft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_drafts_leagues_league_id1",
                table: "drafts");

            migrationBuilder.DropIndex(
                name: "ix_drafts_league_id1",
                table: "drafts");

            migrationBuilder.DropColumn(
                name: "league_id1",
                table: "drafts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "league_id1",
                table: "drafts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_drafts_league_id1",
                table: "drafts",
                column: "league_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_drafts_leagues_league_id1",
                table: "drafts",
                column: "league_id1",
                principalTable: "leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
