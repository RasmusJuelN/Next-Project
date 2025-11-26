using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class SplitQuestionnatireTypesIntoSeparateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantIds",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TeacherFK",
                table: "ActiveQuestionnaire",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StudentFK",
                table: "ActiveQuestionnaire",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CompletedParticipantIds",
                table: "ActiveQuestionnaire",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ParticipantIds",
                table: "ActiveQuestionnaire",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("08062aef-1e18-4c86-ac07-46c9d579e750"),
                columns: new[] { "CompletedParticipantIds", "ParticipantIds" },
                values: new object[] { "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("4814723f-50af-4414-9c17-c79d7aac3831"),
                columns: new[] { "CompletedParticipantIds", "ParticipantIds" },
                values: new object[] { "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("560fa037-52d2-4c8d-86f3-7467ed48f54d"),
                columns: new[] { "CompletedParticipantIds", "ParticipantIds" },
                values: new object[] { "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("76e83a1c-73e4-4572-b0e3-3023f503151f"),
                columns: new[] { "CompletedParticipantIds", "ParticipantIds" },
                values: new object[] { "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("7c81756d-2ae8-41e8-ac79-824bc632c8a1"),
                columns: new[] { "CompletedParticipantIds", "ParticipantIds" },
                values: new object[] { "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"),
                columns: new[] { "CompletedParticipantIds", "ParticipantIds" },
                values: new object[] { "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"),
                columns: new[] { "CompletedParticipantIds", "ParticipantIds" },
                values: new object[] { "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("b812acd2-f43a-42f0-9a10-02158380c88c"),
                columns: new[] { "CompletedParticipantIds", "ParticipantIds" },
                values: new object[] { "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "ActiveQuestionnaire",
                keyColumn: "Id",
                keyValue: new Guid("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"),
                columns: new[] { "CompletedParticipantIds", "ParticipantIds" },
                values: new object[] { "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: -2,
                column: "ParticipantIds",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: -1,
                column: "ParticipantIds",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_User_ParticipantIds",
                table: "User",
                column: "ParticipantIds");

            migrationBuilder.AddForeignKey(
                name: "FK_User_ActiveQuestionnaire_ParticipantIds",
                table: "User",
                column: "ParticipantIds",
                principalTable: "ActiveQuestionnaire",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_ActiveQuestionnaire_ParticipantIds",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ParticipantIds",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ParticipantIds",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CompletedParticipantIds",
                table: "ActiveQuestionnaire");

            migrationBuilder.DropColumn(
                name: "ParticipantIds",
                table: "ActiveQuestionnaire");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherFK",
                table: "ActiveQuestionnaire",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentFK",
                table: "ActiveQuestionnaire",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
