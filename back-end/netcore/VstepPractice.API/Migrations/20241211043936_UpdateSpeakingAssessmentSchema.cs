using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using VstepPractice.API.Models.Entities;

#nullable disable

namespace VstepPractice.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSpeakingAssessmentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "accuracy",
                table: "SpeakingAssessments",
                type: "numeric(4,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "prosody",
                table: "SpeakingAssessments",
                type: "numeric(4,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "topicScore",
                table: "SpeakingAssessments",
                type: "numeric(4,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<List<WordDetail>>(
                name: "wordDetails",
                table: "SpeakingAssessments",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accuracy",
                table: "SpeakingAssessments");

            migrationBuilder.DropColumn(
                name: "prosody",
                table: "SpeakingAssessments");

            migrationBuilder.DropColumn(
                name: "topicScore",
                table: "SpeakingAssessments");

            migrationBuilder.DropColumn(
                name: "wordDetails",
                table: "SpeakingAssessments");
        }
    }
}
