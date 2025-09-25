using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherResponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                keyValue: -27);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -26);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -25);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -21);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -20);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -19);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -15);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -14);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -13);

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -36,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), -8 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -35,
                column: "ActiveQuestionnaireFK",
                value: new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -34,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), -3 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -30,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), -9 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -29,
                column: "ActiveQuestionnaireFK",
                value: new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -28,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), -1 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -24,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), -9 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -23,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), -5 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -22,
                column: "ActiveQuestionnaireFK",
                value: new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -18,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), -7 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -17,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), -4 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -16,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), -2 });

            migrationBuilder.InsertData(
                table: "ActiveQuestionnaireResponse",
                columns: new[] { "Id", "ActiveQuestionnaireFK", "CustomResponse", "Discriminator", "OptionFK", "QuestionFK" },
                values: new object[,]
                {
                    { -63, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireTeacherResponseModel", -7, -3 },
                    { -62, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireTeacherResponseModel", -4, -2 },
                    { -61, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireTeacherResponseModel", -3, -1 },
                    { -60, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireStudentResponseModel", -9, -3 },
                    { -59, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -58, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireStudentResponseModel", -1, -1 },
                    { -57, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireTeacherResponseModel", -7, -3 },
                    { -56, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireTeacherResponseModel", -5, -2 },
                    { -55, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireTeacherResponseModel", -1, -1 },
                    { -54, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -53, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -52, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -3, -1 },
                    { -51, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireTeacherResponseModel", -9, -3 },
                    { -50, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireTeacherResponseModel", -5, -2 },
                    { -49, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireTeacherResponseModel", -2, -1 },
                    { -48, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -47, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireStudentResponseModel", -5, -2 },
                    { -46, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireStudentResponseModel", -3, -1 },
                    { -45, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireTeacherResponseModel", -9, -3 },
                    { -44, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireTeacherResponseModel", -6, -2 },
                    { -43, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireTeacherResponseModel", -3, -1 },
                    { -42, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -41, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -40, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireStudentResponseModel", -2, -1 },
                    { -39, new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), null, "ActiveQuestionnaireTeacherResponseModel", -7, -3 },
                    { -38, new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), null, "ActiveQuestionnaireTeacherResponseModel", -6, -2 },
                    { -37, new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), null, "ActiveQuestionnaireTeacherResponseModel", -1, -1 },
                    { -33, new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), null, "ActiveQuestionnaireTeacherResponseModel", -9, -3 },
                    { -32, new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), null, "ActiveQuestionnaireTeacherResponseModel", -4, -2 },
                    { -31, new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), null, "ActiveQuestionnaireTeacherResponseModel", -2, -1 },
                    { -27, new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), null, "ActiveQuestionnaireTeacherResponseModel", -9, -3 },
                    { -26, new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), null, "ActiveQuestionnaireTeacherResponseModel", -6, -2 },
                    { -25, new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), null, "ActiveQuestionnaireTeacherResponseModel", -2, -1 },
                    { -21, new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), null, "ActiveQuestionnaireTeacherResponseModel", -8, -3 },
                    { -20, new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), null, "ActiveQuestionnaireTeacherResponseModel", -5, -2 },
                    { -19, new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), null, "ActiveQuestionnaireTeacherResponseModel", -2, -1 },
                    { -15, new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), null, "ActiveQuestionnaireTeacherResponseModel", -7, -3 },
                    { -14, new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), null, "ActiveQuestionnaireTeacherResponseModel", -4, -2 },
                    { -13, new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), null, "ActiveQuestionnaireTeacherResponseModel", -3, -1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -63);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -62);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -61);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -60);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -59);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -58);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -57);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -56);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -55);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -54);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -53);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -52);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -51);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -50);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -49);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -48);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -47);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -46);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -45);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -44);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -43);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -42);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -41);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -40);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -39);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -38);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -37);

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
                keyValue: -27);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -26);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -25);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -21);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -20);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -19);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -15);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -14);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -13);

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -36,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), -9 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -35,
                column: "ActiveQuestionnaireFK",
                value: new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -34,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), -1 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -30,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), -7 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -29,
                column: "ActiveQuestionnaireFK",
                value: new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -28,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), -3 });

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
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK" },
                values: new object[] { new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), -6 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -22,
                column: "ActiveQuestionnaireFK",
                value: new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"));

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

            migrationBuilder.InsertData(
                table: "ActiveQuestionnaireResponse",
                columns: new[] { "Id", "ActiveQuestionnaireFK", "CustomResponse", "Discriminator", "OptionFK", "QuestionFK" },
                values: new object[,]
                {
                    { -33, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -32, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -31, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -3, -1 },
                    { -27, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -26, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -25, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireStudentResponseModel", -2, -1 },
                    { -21, new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), null, "ActiveQuestionnaireStudentResponseModel", -9, -3 },
                    { -20, new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), null, "ActiveQuestionnaireStudentResponseModel", -5, -2 },
                    { -19, new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), null, "ActiveQuestionnaireStudentResponseModel", -1, -1 },
                    { -15, new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -14, new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), null, "ActiveQuestionnaireStudentResponseModel", -4, -2 },
                    { -13, new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), null, "ActiveQuestionnaireStudentResponseModel", -2, -1 }
                });
        }
    }
}
