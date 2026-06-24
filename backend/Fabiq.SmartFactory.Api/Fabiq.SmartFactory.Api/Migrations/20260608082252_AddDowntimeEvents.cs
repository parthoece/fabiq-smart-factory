using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fabiq.SmartFactory.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDowntimeEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "downtime_events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    MachineId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LineId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WorkOrderId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReasonCode = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_downtime_events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_downtime_events_EventId",
                table: "downtime_events",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_downtime_events_LineId",
                table: "downtime_events",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_downtime_events_MachineId",
                table: "downtime_events",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_downtime_events_ReasonCode",
                table: "downtime_events",
                column: "ReasonCode");

            migrationBuilder.CreateIndex(
                name: "IX_downtime_events_StartedAt",
                table: "downtime_events",
                column: "StartedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "downtime_events");
        }
    }
}
