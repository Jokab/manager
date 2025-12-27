using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerGame.Core.Migrations
{
    /// <inheritdoc />
    public partial class DraftAggregateParticipantsAndPicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "draft_participant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    draft_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seat = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_draft_participant", x => x.id);
                    table.ForeignKey(
                        name: "fk_draft_participant_drafts_draft_id",
                        column: x => x.draft_id,
                        principalTable: "drafts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "draft_pick",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    draft_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pick_number = table.Column<int>(type: "integer", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    picked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_draft_pick", x => x.id);
                    table.ForeignKey(
                        name: "fk_draft_pick_drafts_draft_id",
                        column: x => x.draft_id,
                        principalTable: "drafts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_draft_participant_draft_id_seat",
                table: "draft_participant",
                columns: new[] { "draft_id", "seat" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_draft_participant_draft_id_team_id",
                table: "draft_participant",
                columns: new[] { "draft_id", "team_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_draft_pick_draft_id_pick_number",
                table: "draft_pick",
                columns: new[] { "draft_id", "pick_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_draft_pick_draft_id_player_id",
                table: "draft_pick",
                columns: new[] { "draft_id", "player_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "draft_participant");

            migrationBuilder.DropTable(
                name: "draft_pick");
        }
    }
}
