using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSortOrderToQuestionsAndOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "QuestionnaireTemplateQuestion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "QuestionnaireTemplateOption",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -9,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -8,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -7,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -6,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -5,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -4,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -3,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -2,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -1,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 1,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 2,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 3,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 4,
                column: "SortOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 5,
                column: "SortOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 6,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 7,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 8,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 9,
                column: "SortOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 10,
                column: "SortOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 11,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 12,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 13,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 14,
                column: "SortOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 15,
                column: "SortOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 16,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 17,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 18,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 19,
                column: "SortOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 20,
                column: "SortOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 21,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 22,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 23,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 24,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 25,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 26,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 27,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 28,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 29,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 30,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 31,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 32,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 33,
                column: "SortOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 34,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 35,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 36,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 37,
                column: "SortOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 38,
                column: "SortOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 39,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 40,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 41,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 42,
                column: "SortOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: -3,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: -2,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: -1,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 1,
                column: "SortOrder",
                value: 0);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 2,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 3,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 4,
                column: "SortOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 5,
                column: "SortOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 6,
                column: "SortOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 7,
                column: "SortOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 8,
                column: "SortOrder",
                value: 7);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 9,
                column: "SortOrder",
                value: 8);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 10,
                column: "SortOrder",
                value: 9);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 11,
                column: "SortOrder",
                value: 10);

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 12,
                column: "SortOrder",
                value: 11);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "QuestionnaireTemplateQuestion");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "QuestionnaireTemplateOption");
        }
    }
}
