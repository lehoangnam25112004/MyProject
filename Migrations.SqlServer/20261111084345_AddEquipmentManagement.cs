using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructionProjectManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    AssignedToId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HandedOverById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HandedOverAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReturnedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignments_AspNetUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignments_AspNetUsers_HandedOverById",
                        column: x => x.HandedOverById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignments_AspNetUsers_ReturnedById",
                        column: x => x.ReturnedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignments_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentAssignments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentCode",
                table: "Equipment",
                column: "EquipmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_AssignedToId",
                table: "EquipmentAssignments",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_EquipmentId",
                table: "EquipmentAssignments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_HandedOverById",
                table: "EquipmentAssignments",
                column: "HandedOverById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_ProjectId",
                table: "EquipmentAssignments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_ReturnedById",
                table: "EquipmentAssignments",
                column: "ReturnedById");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentAssignments_TaskId",
                table: "EquipmentAssignments",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentAssignments");

            migrationBuilder.DropTable(
                name: "Equipment");
        }
    }
}
