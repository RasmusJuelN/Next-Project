using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AdddescriptioncolumntoQuestionnaireTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemplateTitle",
                table: "QuestionnaireTemplate",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireTemplate_TemplateTitle_Id",
                table: "QuestionnaireTemplate",
                newName: "IX_QuestionnaireTemplate_Title_Id");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireTemplate_TemplateTitle",
                table: "QuestionnaireTemplate",
                newName: "IX_QuestionnaireTemplate_Title");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "QuestionnaireTemplate",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "QuestionnaireTemplate");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "QuestionnaireTemplate",
                newName: "TemplateTitle");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireTemplate_Title_Id",
                table: "QuestionnaireTemplate",
                newName: "IX_QuestionnaireTemplate_TemplateTitle_Id");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireTemplate_Title",
                table: "QuestionnaireTemplate",
                newName: "IX_QuestionnaireTemplate_TemplateTitle");
        }
    }
}
