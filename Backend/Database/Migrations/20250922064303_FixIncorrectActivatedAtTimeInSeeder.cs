using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class FixIncorrectActivatedAtTimeInSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2025, 9, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 9, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 9, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("4814723f-50af-4414-9c17-c79d7aac3831"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2025, 6, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 6, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 6, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2024, 3, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 3, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 3, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 9, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 9, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 9, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2025, 3, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 3, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 3, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 6, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 6, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 6, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 3, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 3, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 3, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2024, 6, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 6, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 6, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2024, 9, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 9, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 9, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("4814723f-50af-4414-9c17-c79d7aac3831"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 8, 21, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"),
                columns: new[] { "ActivatedAt", "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2023, 8, 20, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });
        }
    }
}
