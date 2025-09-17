using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ActiveQuestionAndStudentRespondsSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var aq1 = Guid.Parse("6135b27d-37c1-420c-b3ef-39f76649d515");
            var aq2 = Guid.Parse("77a08073-bd8e-45c0-90ea-88dc3f494bf8");
            var aq3 = Guid.Parse("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea");

            migrationBuilder.InsertData(
                table: "ActiveQuestionnaire",
                columns: new[] { "Id", "Description", "GroupId", "QuestionnaireTemplateFK", "StudentCompletedAt", "StudentFK", "TeacherCompletedAt", "TeacherFK", "Title" },
                values: new object[,]
                {
                    { new Guid("6135b27d-37c1-420c-b3ef-39f76649d515"), "Description for the new template", new Guid("e66e1023-be16-4cf6-ada2-b5d1f5dbbf59"), new Guid("569e97ba-40ce-4d27-00f5-08ddd8c9910c"), new DateTime(2025, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 1, new DateTime(2025, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 2, "Bedste Land" },
                    { new Guid("77a08073-bd8e-45c0-90ea-88dc3f494bf8"), "Description for the new template", new Guid("e66e1023-be16-4cf6-ada2-b5d1f5dbbf59"), new Guid("569e97ba-40ce-4d27-00f5-08ddd8c9910c"), new DateTime(2025, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 1, new DateTime(2025, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 2, "Bedste Land" },
                    { new Guid("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea"), "Description for the new template", new Guid("e66e1023-be16-4cf6-ada2-b5d1f5dbbf59"), new Guid("569e97ba-40ce-4d27-00f5-08ddd8c9910c"), new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 1, new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 2, "Bedste Land" }
                });

            migrationBuilder.InsertData(
                table: "ActiveQuestionnaireResponse",
                columns: new[] { "Id", "ActiveQuestionnaireFK", "OptionFK", "QuestionFK", "Discriminator" },
                values: new object[,]
                {
                    { -9, aq1, 44, 13, "ActiveQuestionnaireStudentResponseModel" },
                    { -8, aq1, 47, 14, "ActiveQuestionnaireStudentResponseModel" }, // previously only parent changed; set nulls if allowed
                    { -7, aq1, 56, 15, "ActiveQuestionnaireStudentResponseModel" },
                    { -6, aq2, 43, 13, "ActiveQuestionnaireStudentResponseModel" },
                    { -5, aq2, 49, 14, "ActiveQuestionnaireStudentResponseModel" }, // see note above
                    { -4, aq2, 55, 15, "ActiveQuestionnaireStudentResponseModel" },
                    { -3, aq3, 45, 13, "ActiveQuestionnaireStudentResponseModel" },
                    { -2, aq3, 48, 14, "ActiveQuestionnaireStudentResponseModel" }, // see note above
                    { -1, aq3, 52, 15, "ActiveQuestionnaireStudentResponseModel" },
                });


            migrationBuilder.InsertData(
                table: "ActiveQuestionnaireResponse",
                columns: new[] { "Id", "ActiveQuestionnaireFK", "OptionFK", "QuestionFK", "Discriminator" },
                values: new object[,]
                {
                    { -18, aq1, 44, 13, "ActiveQuestionnaireTeacherResponseModel" },
                    { -17, aq1, 47, 14, "ActiveQuestionnaireTeacherResponseModel" }, // previously only parent changed; set nulls if allowed
                    { -16, aq1, 56, 15, "ActiveQuestionnaireTeacherResponseModel" },
                    { -15, aq2, 43, 13, "ActiveQuestionnaireTeacherResponseModel" },
                    { -14, aq2, 49, 14, "ActiveQuestionnaireTeacherResponseModel" }, // see note above
                    { -13, aq2, 55, 15, "ActiveQuestionnaireTeacherResponseModel" },
                    { -12, aq3, 45, 13, "ActiveQuestionnaireTeacherResponseModel" },
                    { -11, aq3, 48, 14, "ActiveQuestionnaireTeacherResponseModel" }, // see note above
                    { -10, aq3, 52, 15, "ActiveQuestionnaireTeacherResponseModel" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // First remove the inserted responses
            migrationBuilder.DeleteData("ActiveQuestionnaireResponse", "Id", -9);
            migrationBuilder.DeleteData("ActiveQuestionnaireResponse", "Id", -8);
            migrationBuilder.DeleteData("ActiveQuestionnaireResponse", "Id", -7);
            migrationBuilder.DeleteData("ActiveQuestionnaireResponse", "Id", -6);
            migrationBuilder.DeleteData("ActiveQuestionnaireResponse", "Id", -5);
            migrationBuilder.DeleteData("ActiveQuestionnaireResponse", "Id", -4);
            migrationBuilder.DeleteData("ActiveQuestionnaireResponse", "Id", -3);
            migrationBuilder.DeleteData("ActiveQuestionnaireResponse", "Id", -2);
            migrationBuilder.DeleteData("ActiveQuestionnaireResponse", "Id", -1);

            // Then remove the three parents (as you already do)
            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: Guid.Parse("6135b27d-37c1-420c-b3ef-39f76649d515"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: Guid.Parse("77a08073-bd8e-45c0-90ea-88dc3f494bf8"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: Guid.Parse("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea"));
        }

    }
}
