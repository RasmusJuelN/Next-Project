using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddResponseCountTrackingForAnonymousQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymousQuesionnaireResponse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionFK = table.Column<int>(type: "int", nullable: false),
                    OptionFK = table.Column<int>(type: "int", nullable: true),
                    ResponseCount = table.Column<int>(type: "int", nullable: false),
                    ActiveQuestionnaireFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActiveQuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymousQuesionnaireResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnonymousQuesionnaireResponse_ActiveQuestionnaire_ActiveQuestionnaireId",
                        column: x => x.ActiveQuestionnaireId,
                        principalTable: "ActiveQuestionnaire",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnonymousQuesionnaireResponse_QuestionnaireTemplateOption_OptionFK",
                        column: x => x.OptionFK,
                        principalTable: "QuestionnaireTemplateOption",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AnonymousQuesionnaireResponse_QuestionnaireTemplateQuestion_QuestionFK",
                        column: x => x.QuestionFK,
                        principalTable: "QuestionnaireTemplateQuestion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousQuesionnaireResponse_ActiveQuestionnaireId",
                table: "AnonymousQuesionnaireResponse",
                column: "ActiveQuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousQuesionnaireResponse_OptionFK",
                table: "AnonymousQuesionnaireResponse",
                column: "OptionFK");

            migrationBuilder.CreateIndex(
                name: "IX_AnonymousQuesionnaireResponse_QuestionFK",
                table: "AnonymousQuesionnaireResponse",
                column: "QuestionFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymousQuesionnaireResponse");
        }
    }
}
