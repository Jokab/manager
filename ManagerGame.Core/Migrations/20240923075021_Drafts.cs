using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerGame.Core.Migrations
{
    /// <inheritdoc />
    public partial class Drafts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "league_id",
                table: "teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "league",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_league", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "drafts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    league_id = table.Column<Guid>(type: "uuid", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    DraftOrderCurrent = table.Column<int>(type: "integer", nullable: false),
                    DraftOrderPrevious = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drafts", x => x.id);
                    table.ForeignKey(
                        name: "fk_drafts_league_league_id",
                        column: x => x.league_id,
                        principalTable: "league",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_teams_league_id",
                table: "teams",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "ix_drafts_league_id",
                table: "drafts",
                column: "league_id");

            migrationBuilder.AddForeignKey(
                name: "fk_teams_league_league_id",
                table: "teams",
                column: "league_id",
                principalTable: "league",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_teams_league_league_id",
                table: "teams");

            migrationBuilder.DropTable(
                name: "drafts");

            migrationBuilder.DropTable(
                name: "league");

            migrationBuilder.DropIndex(
                name: "ix_teams_league_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "league_id",
                table: "teams");
        }
    }
}
