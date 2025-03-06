using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class SplitActiveQuestionnaireReponseModelintoseparateentitiesforstudentandteacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomStudentResponse",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "CustomTeacherResponse",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "StudentResponse",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.RenameColumn(
                name: "TeacherResponse",
                table: "ActiveQuestionnaireResponse",
                newName: "CustomResponse");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ActiveQuestionnaireResponse",
                type: "nvarchar(55)",
                maxLength: 55,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OptionId",
                table: "ActiveQuestionnaireResponse",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "ActiveQuestionnaireResponse",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "OptionId",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.RenameColumn(
                name: "CustomResponse",
                table: "ActiveQuestionnaireResponse",
                newName: "TeacherResponse");

            migrationBuilder.AddColumn<string>(
                name: "CustomStudentResponse",
                table: "ActiveQuestionnaireResponse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomTeacherResponse",
                table: "ActiveQuestionnaireResponse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "ActiveQuestionnaireResponse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentResponse",
                table: "ActiveQuestionnaireResponse",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
