using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangedOneDateForSeedDateToBeOneYearBehind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea"),
                columns: new[] { "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2024, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2024, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea"),
                columns: new[] { "StudentCompletedAt", "TeacherCompletedAt" },
                values: new object[] { new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158), new DateTime(2025, 8, 22, 9, 58, 30, 536, DateTimeKind.Unspecified).AddTicks(158) });
        }
    }
}
