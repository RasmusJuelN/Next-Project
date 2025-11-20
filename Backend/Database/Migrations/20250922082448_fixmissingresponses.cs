using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class fixmissingresponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -27,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), -7 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -26,
                column: "ActiveQuestionnaireFK",
                value: new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -25,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), -2 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -24,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), -8 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -23,
                column: "ActiveQuestionnaireFK",
                value: new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -22,
                column: "ActiveQuestionnaireFK",
                value: new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -21,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), -9 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -20,
                column: "ActiveQuestionnaireFK",
                value: new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -19,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), -1 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -18,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), -9 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -17,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), -5 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -16,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), -3 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -15,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), -7 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -14,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), -4 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -13,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), -2 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -12,
                column: "ActiveQuestionnaireFK",
                value: new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -11,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), -6 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -10,
                column: "ActiveQuestionnaireFK",
                value: new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"));

            migrationBuilder.InsertData(
                table: "ActiveQuestionnaireResponse",
                columns: new[] { "Id", "ActiveQuestionnaireFK", "CustomResponse", "Discriminator", "OptionFK", "QuestionFK" },
                values: new object[,]
                {
                    { -36, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireStudentResponseModel", -9, -3 },
                    { -35, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -34, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireStudentResponseModel", -1, -1 },
                    { -33, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -32, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -31, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -3, -1 },
                    { -30, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -29, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireStudentResponseModel", -5, -2 },
                    { -28, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireStudentResponseModel", -3, -1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -36);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -35);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -34);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -33);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -32);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -31);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -30);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -29);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -28);

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -27,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), -9 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -26,
                column: "ActiveQuestionnaireFK",
                value: new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -25,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), -1 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -24,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), -7 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -23,
                column: "ActiveQuestionnaireFK",
                value: new Guid("4814723f-50af-4414-9c17-c79d7aac3831"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -22,
                column: "ActiveQuestionnaireFK",
                value: new Guid("4814723f-50af-4414-9c17-c79d7aac3831"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -21,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), -7 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -20,
                column: "ActiveQuestionnaireFK",
                value: new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -19,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), -3 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -18,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), -7 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -17,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), -6 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -16,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), -2 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -15,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), -8 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -14,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), -6 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -13,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), -3 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -12,
                column: "ActiveQuestionnaireFK",
                value: new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -11,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), -5 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -10,
                column: "ActiveQuestionnaireFK",
                value: new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"));

            migrationBuilder.InsertData(
                table: "ActiveQuestionnaireResponse",
                columns: new[] { "Id", "ActiveQuestionnaireFK", "CustomResponse", "Discriminator", "OptionFK", "QuestionFK" },
                values: new object[,]
                {
                    { -9, new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), null, "ActiveQuestionnaireStudentResponseModel", -9, -3 },
                    { -8, new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), null, "ActiveQuestionnaireStudentResponseModel", -5, -2 },
                    { -7, new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), null, "ActiveQuestionnaireStudentResponseModel", -3, -1 },
                    { -6, new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -5, new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), null, "ActiveQuestionnaireStudentResponseModel", -4, -2 },
                    { -4, new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), null, "ActiveQuestionnaireStudentResponseModel", -2, -1 },
                    { -3, new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), null, "ActiveQuestionnaireStudentResponseModel", -9, -3 },
                    { -2, new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -1, new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), null, "ActiveQuestionnaireStudentResponseModel", -1, -1 }
                });
        }
    }
}
