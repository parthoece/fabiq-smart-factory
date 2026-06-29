using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fabiq.SmartFactory.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maintenance_alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlertId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    MachineId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WorkOrderId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AlertType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Severity = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SourceEventId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    TemperatureC = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    VibrationMmS = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    CycleTimeSeconds = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    ErrorCount = table.Column<int>(type: "integer", nullable: true),
                    ScrapRate = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maintenance_alerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_alerts_AlertId",
                table: "maintenance_alerts",
                column: "AlertId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_alerts_AlertType",
                table: "maintenance_alerts",
                column: "AlertType");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_alerts_CreatedAt",
                table: "maintenance_alerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_alerts_MachineId",
                table: "maintenance_alerts",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_alerts_ResolvedAt",
                table: "maintenance_alerts",
                column: "ResolvedAt");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_alerts_Severity",
                table: "maintenance_alerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_alerts_WorkOrderId",
                table: "maintenance_alerts",
                column: "WorkOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "maintenance_alerts");
        }
    }
}
