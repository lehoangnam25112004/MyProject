using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructionProjectManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddConstructionLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConstructionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Weather = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Workforce = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WorkPerformed = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MaterialsUsed = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    EquipmentUsed = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Issues = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AttachmentFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    AttachmentStorageName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstructionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConstructionLogs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConstructionLogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConstructionLogs_CreatedById",
                table: "ConstructionLogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ConstructionLogs_ProjectId_LogDate",
                table: "ConstructionLogs",
                columns: new[] { "ProjectId", "LogDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConstructionLogs");
        }
    }
}
