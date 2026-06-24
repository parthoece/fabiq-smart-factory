using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fabiq.SmartFactory.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "production_events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    MachineId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WorkOrderId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LineId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EventType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PartId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    DefectType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production_events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_production_events_CreatedAt",
                table: "production_events",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_production_events_EventId",
                table: "production_events",
                column: "EventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "production_events");
        }
    }
}
