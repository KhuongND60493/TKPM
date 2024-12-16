using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VstepPractice.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNameInSpeakingAssessments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "finalScore",
                table: "StudentAttempts",
                newName: "finalScore");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "finalScore",
                table: "StudentAttempts",
                newName: "finalScore");
        }
    }
}
