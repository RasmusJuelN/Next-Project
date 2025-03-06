using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddforeignkeyconstrainstoActiveQuestionnaireResponseBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "ActiveQuestionnaireResponse",
                newName: "QuestionFK");

            migrationBuilder.RenameColumn(
                name: "OptionId",
                table: "ActiveQuestionnaireResponse",
                newName: "OptionFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_OptionFK",
                table: "ActiveQuestionnaireResponse",
                column: "OptionFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_QuestionFK",
                table: "ActiveQuestionnaireResponse",
                column: "QuestionFK");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateOption_OptionFK",
                table: "ActiveQuestionnaireResponse",
                column: "OptionFK",
                principalTable: "QuestionnaireTemplateOption",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateQuestion_QuestionFK",
                table: "ActiveQuestionnaireResponse",
                column: "QuestionFK",
                principalTable: "QuestionnaireTemplateQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateOption_OptionFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateQuestion_QuestionFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropIndex(
                name: "IX_ActiveQuestionnaireResponse_OptionFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropIndex(
                name: "IX_ActiveQuestionnaireResponse_QuestionFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.RenameColumn(
                name: "QuestionFK",
                table: "ActiveQuestionnaireResponse",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "OptionFK",
                table: "ActiveQuestionnaireResponse",
                newName: "OptionId");
        }
    }
}
