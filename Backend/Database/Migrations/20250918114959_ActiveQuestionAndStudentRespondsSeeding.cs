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
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateQuestion_QuestionFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("6135b27d-37c1-420c-b3ef-39f76649d515"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("77a08073-bd8e-45c0-90ea-88dc3f494bf8"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea"));

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -9,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), -9, -3 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -8,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), -5, -2 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -7,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), -3, -1 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -6,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), -7, -3 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -5,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), -4, -2 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), -2, -1 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), -9, -3 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), -6, -2 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), -1, -1 });

            migrationBuilder.InsertData(
                table: "QuestionnaireTemplate",
                columns: new[] { "Id", "CreatedAt", "Description", "LastUpated", "TemplateStatus", "Title" },
                values: new object[] { new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2025, 8, 19, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new DateTime(2025, 8, 19, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 1, "Bedste Land" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Discriminator", "FullName", "Guid", "Permissions", "PrimaryRole", "UserName" },
                values: new object[,]
                {
                    { -2, "TeacherModel", "Teacher One", new Guid("76889b1b-c762-482b-a1cb-2e4189dbb484"), 24, "Teacher", "teacher1" },
                    { -1, "StudentModel", "Student One", new Guid("8ccdeb24-9027-40f0-986f-a5d2d171469a"), 16, "Student", "student1" }
                });

            migrationBuilder.InsertData(
                table: "QuestionnaireGroups",
                columns: new[] { "GroupId", "CreatedAt", "Name", "TemplateId" },
                values: new object[] { new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new DateTime(2025, 8, 19, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Default Group", new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf") });

            migrationBuilder.InsertData(
                table: "QuestionnaireTemplateQuestion",
                columns: new[] { "Id", "AllowCustom", "Prompt", "QuestionnaireTemplateFK" },
                values: new object[,]
                {
                    { -3, false, "Østeuropa", new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf") },
                    { -2, false, "Skandinavien", new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf") },
                    { -1, false, "Asien", new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf") }
                });

            migrationBuilder.InsertData(
                table: "ActiveQuestionnaire",
                columns: new[] { "Id", "ActivatedAt", "Description", "GroupId", "QuestionnaireTemplateFK", "StudentCompletedAt", "StudentFK", "TeacherCompletedAt", "TeacherFK", "Title" },
                values: new object[,]
                {
                    { new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -1, new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -2, "Bedste Land" },
                    { new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2025, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -1, new DateTime(2025, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -2, "Bedste Land" },
                    { new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2024, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -1, new DateTime(2024, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -2, "Bedste Land" },
                    { new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2023, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -1, new DateTime(2023, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -2, "Bedste Land" },
                    { new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2025, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -1, new DateTime(2025, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -2, "Bedste Land" },
                    { new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2023, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -1, new DateTime(2023, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -2, "Bedste Land" },
                    { new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -1, new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -2, "Bedste Land" },
                    { new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2024, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -1, new DateTime(2024, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -2, "Bedste Land" },
                    { new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), "Description for the new template.", new Guid("310c585d-0c9a-4679-802e-1c1538475636"), new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"), new DateTime(2024, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -1, new DateTime(2024, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), -2, "Bedste Land" }
                });

            migrationBuilder.InsertData(
                table: "QuestionnaireTemplateOption",
                columns: new[] { "Id", "DisplayText", "OptionValue", "QuestionFK" },
                values: new object[,]
                {
                    { -9, "Bulgarien", -9, -3 },
                    { -8, "Polen", -8, -3 },
                    { -7, "Rusland", -7, -3 },
                    { -6, "Sverige", -6, -2 },
                    { -5, "Norge", -5, -2 },
                    { -4, "Danmark", -4, -2 },
                    { -3, "SydKorea", -3, -1 },
                    { -2, "Indien", -2, -1 },
                    { -1, "Japan", -1, -1 }
                });

            migrationBuilder.InsertData(
                table: "ActiveQuestionnaireResponse",
                columns: new[] { "Id", "ActiveQuestionnaireFK", "CustomResponse", "Discriminator", "OptionFK", "QuestionFK" },
                values: new object[,]
                {
                    { -27, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireStudentResponseModel", -9, -3 },
                    { -26, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -25, new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"), null, "ActiveQuestionnaireStudentResponseModel", -1, -1 },
                    { -24, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -23, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -22, new Guid("4814723f-50af-4414-9c17-c79d7aac3831"), null, "ActiveQuestionnaireStudentResponseModel", -3, -1 },
                    { -21, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -20, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireStudentResponseModel", -5, -2 },
                    { -19, new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"), null, "ActiveQuestionnaireStudentResponseModel", -3, -1 },
                    { -18, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireStudentResponseModel", -7, -3 },
                    { -17, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -16, new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"), null, "ActiveQuestionnaireStudentResponseModel", -2, -1 },
                    { -15, new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), null, "ActiveQuestionnaireStudentResponseModel", -8, -3 },
                    { -14, new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), null, "ActiveQuestionnaireStudentResponseModel", -6, -2 },
                    { -13, new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"), null, "ActiveQuestionnaireStudentResponseModel", -3, -1 },
                    { -12, new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), null, "ActiveQuestionnaireStudentResponseModel", -9, -3 },
                    { -11, new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), null, "ActiveQuestionnaireStudentResponseModel", -5, -2 },
                    { -10, new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"), null, "ActiveQuestionnaireStudentResponseModel", -1, -1 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateQuestion_QuestionFK",
                table: "ActiveQuestionnaireResponse",
                column: "QuestionFK",
                principalTable: "QuestionnaireTemplateQuestion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateQuestion_QuestionFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"));

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
                keyValue: -24);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -23);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -22);

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
                keyValue: -18);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -17);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -16);

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

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -12);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -11);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -10);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("4814723f-50af-4414-9c17-c79d7aac3831"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"));

            migrationBuilder.DeleteData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"));

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -9);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "QuestionnaireGroups",
                keyColumn: "GroupId",
                keyValue: new Guid("310c585d-0c9a-4679-802e-1c1538475636"));

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplate",
                keyColumn: "Id",
                keyValue: new Guid("69088ed6-4fa5-4e85-8d80-18334b7bfabf"));

            migrationBuilder.InsertData(
                table: "ActiveQuestionnaire",
                columns: new[] { "Id", "Description", "GroupId", "QuestionnaireTemplateFK", "StudentCompletedAt", "StudentFK", "TeacherCompletedAt", "TeacherFK", "Title" },
                values: new object[,]
                {
                    { new Guid("6135b27d-37c1-420c-b3ef-39f76649d515"), "Description for the new template", new Guid("e66e1023-be16-4cf6-ada2-b5d1f5dbbf59"), new Guid("569e97ba-40ce-4d27-00f5-08ddd8c9910c"), new DateTime(2025, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 1, new DateTime(2025, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 2, "Bedste Land" },
                    { new Guid("77a08073-bd8e-45c0-90ea-88dc3f494bf8"), "Description for the new template", new Guid("e66e1023-be16-4cf6-ada2-b5d1f5dbbf59"), new Guid("569e97ba-40ce-4d27-00f5-08ddd8c9910c"), new DateTime(2025, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 1, new DateTime(2025, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 2, "Bedste Land" },
                    { new Guid("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea"), "Description for the new template", new Guid("e66e1023-be16-4cf6-ada2-b5d1f5dbbf59"), new Guid("569e97ba-40ce-4d27-00f5-08ddd8c9910c"), new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 1, new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), 2, "Bedste Land" }
                });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -9,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("6135b27d-37c1-420c-b3ef-39f76649d515"), 44, 13 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -8,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("77a08073-bd8e-45c0-90ea-88dc3f494bf8"), 48, 14 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -7,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea"), 56, 15 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -6,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("6135b27d-37c1-420c-b3ef-39f76649d515"), 43, 13 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -5,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("77a08073-bd8e-45c0-90ea-88dc3f494bf8"), 49, 14 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea"), 55, 15 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("6135b27d-37c1-420c-b3ef-39f76649d515"), 45, 13 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("77a08073-bd8e-45c0-90ea-88dc3f494bf8"), 47, 14 });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaireResponse",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ActiveQuestionnaireFK", "OptionFK", "QuestionFK" },
                values: new object[] { new Guid("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea"), 52, 15 });

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateQuestion_QuestionFK",
                table: "ActiveQuestionnaireResponse",
                column: "QuestionFK",
                principalTable: "QuestionnaireTemplateQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
