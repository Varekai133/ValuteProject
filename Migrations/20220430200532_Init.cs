using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSRProject.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Valutes",
                columns: table => new
                {
                    ValuteId = table.Column<string>(type: "TEXT", nullable: false),
                    NumCode = table.Column<int>(type: "INTEGER", nullable: false),
                    CharCode = table.Column<string>(type: "TEXT", nullable: false),
                    Nominal = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Valutes", x => x.ValuteId);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<float>(type: "REAL", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValuteId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Courses_Valutes_ValuteId",
                        column: x => x.ValuteId,
                        principalTable: "Valutes",
                        principalColumn: "ValuteId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ValuteId",
                table: "Courses",
                column: "ValuteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Valutes");
        }
    }
}
