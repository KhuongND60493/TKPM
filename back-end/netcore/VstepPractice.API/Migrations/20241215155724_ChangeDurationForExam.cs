using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VstepPractice.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDurationForExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "duration",
                table: "StudentAttempts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "duration",
                table: "Exams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StudentAttemptDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    startTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    endTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    sectionId = table.Column<int>(type: "integer", nullable: false),
                    sectionType = table.Column<int>(type: "integer", nullable: false),
                    duration = table.Column<int>(type: "integer", nullable: false),
                    studentAttemptId = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttemptDetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentAttemptDetails_StudentAttempts_studentAttemptId",
                        column: x => x.studentAttemptId,
                        principalTable: "StudentAttempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttemptDetails_studentAttemptId",
                table: "StudentAttemptDetails",
                column: "studentAttemptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAttemptDetails");

            migrationBuilder.DropColumn(
                name: "duration",
                table: "StudentAttempts");

            migrationBuilder.DropColumn(
                name: "duration",
                table: "Exams");

        }
    }
}
