using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Explicitlydefinethekeyrelationshipsanddeletebehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaire_ActiveQuestionnaireFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaire_ActiveQuestionnaireFK",
                table: "ActiveQuestionnaireResponse",
                column: "ActiveQuestionnaireFK",
                principalTable: "ActiveQuestionnaire",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaire_ActiveQuestionnaireFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaire_ActiveQuestionnaireFK",
                table: "ActiveQuestionnaireResponse",
                column: "ActiveQuestionnaireFK",
                principalTable: "ActiveQuestionnaire",
                principalColumn: "Id");
        }
    }
}
