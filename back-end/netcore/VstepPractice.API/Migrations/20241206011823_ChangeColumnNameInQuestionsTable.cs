using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VstepPractice.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNameInQuestionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_SectionParts_sectionId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "sectionId",
                table: "Questions",
                newName: "passageId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_sectionId",
                table: "Questions",
                newName: "IX_Questions_passageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_SectionParts_passageId",
                table: "Questions",
                column: "passageId",
                principalTable: "SectionParts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_SectionParts_passageId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "passageId",
                table: "Questions",
                newName: "sectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_passageId",
                table: "Questions",
                newName: "IX_Questions_sectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_SectionParts_sectionId",
                table: "Questions",
                column: "sectionId",
                principalTable: "SectionParts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
