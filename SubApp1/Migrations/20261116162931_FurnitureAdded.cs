using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubApp1.Migrations
{
    /// <inheritdoc />
    public partial class FurnitureAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FurnitureItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurnitureItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentFurnitures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    FurnitureId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFurnitures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentFurnitures_FurnitureItems_FurnitureId",
                        column: x => x.FurnitureId,
                        principalTable: "FurnitureItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureItems_Slug",
                table: "FurnitureItems",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentFurnitures_FurnitureId",
                table: "StudentFurnitures",
                column: "FurnitureId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFurnitures_StudentId_FurnitureId",
                table: "StudentFurnitures",
                columns: new[] { "StudentId", "FurnitureId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentFurnitures");

            migrationBuilder.DropTable(
                name: "FurnitureItems");
        }
    }
}
