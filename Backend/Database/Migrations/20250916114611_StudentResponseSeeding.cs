using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class StudentResponseSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ActiveQuestionnaireResponse",
                columns: new[] { "Id", "ActiveQuestionnaireFK", "CustomResponse", "Discriminator", "OptionFK", "QuestionFK" },
                values: new object[,]
                {
                    { -9, new Guid("3bea93c1-7c90-4c26-e329-08dddf06e1a3"), null, "ActiveQuestionnaireStudentResponseModel", 56, 15 },
                    { -8, new Guid("3bea93c1-7c90-4c26-e329-08dddf06e1a3"), null, "ActiveQuestionnaireStudentResponseModel", 48, 14 },
                    { -7, new Guid("3bea93c1-7c90-4c26-e329-08dddf06e1a3"), null, "ActiveQuestionnaireStudentResponseModel", 44, 13 },
                    { -6, new Guid("3bea93c1-7c90-4c26-e329-08dddf06e1a3"), null, "ActiveQuestionnaireStudentResponseModel", 55, 15 },
                    { -5, new Guid("3bea93c1-7c90-4c26-e329-08dddf06e1a3"), null, "ActiveQuestionnaireStudentResponseModel", 49, 14 },
                    { -4, new Guid("3bea93c1-7c90-4c26-e329-08dddf06e1a3"), null, "ActiveQuestionnaireStudentResponseModel", 43, 13 },
                    { -3, new Guid("3bea93c1-7c90-4c26-e329-08dddf06e1a3"), null, "ActiveQuestionnaireStudentResponseModel", 52, 15 },
                    { -2, new Guid("3bea93c1-7c90-4c26-e329-08dddf06e1a3"), null, "ActiveQuestionnaireStudentResponseModel", 47, 14 },
                    { -1, new Guid("3bea93c1-7c90-4c26-e329-08dddf06e1a3"), null, "ActiveQuestionnaireStudentResponseModel", 45, 13 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -9);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -1);
        }
    }
}
