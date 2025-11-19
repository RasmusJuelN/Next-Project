using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentQuestionnaireResponseSeederToUseMockUserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "FullName", "Guid", "UserName" },
                values: new object[] { "Goodman Powell", new Guid("2fd332b4-39b0-4d4b-9e52-04cd9eea2dd4"), "Goodman.Powell" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "FullName", "Guid", "UserName" },
                values: new object[] { "Brigitte Cantrell", new Guid("7f8669b1-82ef-4992-b129-98816c8062ae"), "Brigitte.Cantrell" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "FullName", "Guid", "UserName" },
                values: new object[] { "Teacher One", new Guid("76889b1b-c762-482b-a1cb-2e4189dbb484"), "teacher1" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "FullName", "Guid", "UserName" },
                values: new object[] { "Student One", new Guid("8ccdeb24-9027-40f0-986f-a5d2d171469a"), "student1" });
        }
    }
}
