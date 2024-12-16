using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VstepPractice.API.Migrations
{
    /// <inheritdoc />
    public partial class initDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    firstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    lastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    userId = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.id);
                    table.ForeignKey(
                        name: "FK_Exams_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SectionParts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    instructions = table.Column<string>(type: "text", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    orderNum = table.Column<int>(type: "integer", nullable: false),
                    sectionType = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    examId = table.Column<int>(type: "integer", nullable: false),
                    parentId = table.Column<int>(type: "integer", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionParts", x => x.id);
                    table.ForeignKey(
                        name: "FK_SectionParts_Exams_examId",
                        column: x => x.examId,
                        principalTable: "Exams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionParts_SectionParts_parentId",
                        column: x => x.parentId,
                        principalTable: "SectionParts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAttempts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userId = table.Column<int>(type: "integer", nullable: false),
                    examId = table.Column<int>(type: "integer", nullable: false),
                    startTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttempts", x => x.id);
                    table.ForeignKey(
                        name: "FK_StudentAttempts_Exams_examId",
                        column: x => x.examId,
                        principalTable: "Exams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StudentAttempts_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sectionId = table.Column<int>(type: "integer", nullable: false),
                    questionText = table.Column<string>(type: "text", nullable: true),
                    point = table.Column<decimal>(type: "numeric", nullable: false),
                    orderNum = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Questions_SectionParts_sectionId",
                        column: x => x.sectionId,
                        principalTable: "SectionParts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    questionId = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    isCorrect = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    orderNum = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_questionId",
                        column: x => x.questionId,
                        principalTable: "Questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    attemptId = table.Column<int>(type: "integer", nullable: false),
                    questionId = table.Column<int>(type: "integer", nullable: false),
                    questionOptionId = table.Column<int>(type: "integer", nullable: true),
                    essayAnswer = table.Column<string>(type: "text", nullable: true),
                    aiFeedback = table.Column<string>(type: "text", nullable: true),
                    score = table.Column<decimal>(type: "numeric", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.id);
                    table.ForeignKey(
                        name: "FK_Answers_QuestionOptions_questionOptionId",
                        column: x => x.questionOptionId,
                        principalTable: "QuestionOptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_questionId",
                        column: x => x.questionId,
                        principalTable: "Questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answers_StudentAttempts_attemptId",
                        column: x => x.attemptId,
                        principalTable: "StudentAttempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SpeakingAssessments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    answerId = table.Column<int>(type: "integer", nullable: false),
                    pronunciation = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    fluency = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    vocabulary = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    grammar = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    detailedFeedback = table.Column<string>(type: "text", nullable: false),
                    transcribedText = table.Column<string>(type: "text", nullable: false),
                    audioUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    assessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeakingAssessments", x => x.id);
                    table.ForeignKey(
                        name: "FK_SpeakingAssessments_Answers_answerId",
                        column: x => x.answerId,
                        principalTable: "Answers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WritingAssessments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    answerId = table.Column<int>(type: "integer", nullable: false),
                    taskAchievement = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    coherenceCohesion = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    lexicalResource = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    grammarAccuracy = table.Column<decimal>(type: "numeric(4,2)", nullable: false),
                    detailedFeedback = table.Column<string>(type: "text", nullable: false),
                    assessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingAssessments", x => x.id);
                    table.ForeignKey(
                        name: "FK_WritingAssessments_Answers_answerId",
                        column: x => x.answerId,
                        principalTable: "Answers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_attemptId",
                table: "Answers",
                column: "attemptId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_questionId",
                table: "Answers",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_questionOptionId",
                table: "Answers",
                column: "questionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_userId",
                table: "Exams",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_questionId",
                table: "QuestionOptions",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_sectionId",
                table: "Questions",
                column: "sectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionParts_examId",
                table: "SectionParts",
                column: "examId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionParts_parentId",
                table: "SectionParts",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeakingAssessments_answerId",
                table: "SpeakingAssessments",
                column: "answerId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttempts_examId",
                table: "StudentAttempts",
                column: "examId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttempts_userId",
                table: "StudentAttempts",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingAssessments_answerId",
                table: "WritingAssessments",
                column: "answerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeakingAssessments");

            migrationBuilder.DropTable(
                name: "WritingAssessments");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "StudentAttempts");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "SectionParts");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
