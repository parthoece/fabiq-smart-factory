using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fabiq.SmartFactory.Api.Migrations
{
    public partial class AddMachineStatusEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "machine_status_events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    MachineId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LineId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CurrentWorkOrderId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_machine_status_events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_machine_status_events_CreatedAt",
                table: "machine_status_events",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_machine_status_events_EventId",
                table: "machine_status_events",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_machine_status_events_MachineId",
                table: "machine_status_events",
                column: "MachineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "machine_status_events");
        }
    }
}