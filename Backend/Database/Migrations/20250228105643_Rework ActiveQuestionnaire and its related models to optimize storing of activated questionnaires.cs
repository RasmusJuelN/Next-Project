using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ReworkActiveQuestionnaireanditsrelatedmodelstooptimizestoringofactivatedquestionnaires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaireQuestion_ActiveQuestionnaireQuestionFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomStudentResponseFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomTeacherResponseFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaireOption");

            migrationBuilder.DropTable(
                name: "CustomAnswer");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaireQuestion");

            migrationBuilder.DropIndex(
                name: "IX_ActiveQuestionnaireResponse_ActiveQuestionnaireQuestionFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropIndex(
                name: "IX_ActiveQuestionnaireResponse_CustomStudentResponseFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropIndex(
                name: "IX_ActiveQuestionnaireResponse_CustomTeacherResponseFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "QuestionnaireTemplate");

            migrationBuilder.DropColumn(
                name: "ActiveQuestionnaireQuestionFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "CustomStudentResponseFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "CustomTeacherResponseFK",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                table: "TrackedRefreshToken",
                newName: "ValidUntil");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "QuestionnaireTemplate",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomStudentResponse",
                table: "ActiveQuestionnaireResponse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomTeacherResponse",
                table: "ActiveQuestionnaireResponse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "ActiveQuestionnaireResponse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomStudentResponse",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "CustomTeacherResponse",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.RenameColumn(
                name: "ValidUntil",
                table: "TrackedRefreshToken",
                newName: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "QuestionnaireTemplate",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "QuestionnaireTemplate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ActiveQuestionnaireQuestionFK",
                table: "ActiveQuestionnaireResponse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomStudentResponseFK",
                table: "ActiveQuestionnaireResponse",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomTeacherResponseFK",
                table: "ActiveQuestionnaireResponse",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActiveQuestionnaireQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActiveQuestionnaireFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowCustom = table.Column<bool>(type: "bit", nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaireQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireQuestion_ActiveQuestionnaire_ActiveQuestionnaireFK",
                        column: x => x.ActiveQuestionnaireFK,
                        principalTable: "ActiveQuestionnaire",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomAnswer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActiveQuestionnaireOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActiveQuestionnaireQuestionFK = table.Column<int>(type: "int", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionValue = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaireOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireOption_ActiveQuestionnaireQuestion_ActiveQuestionnaireQuestionFK",
                        column: x => x.ActiveQuestionnaireQuestionFK,
                        principalTable: "ActiveQuestionnaireQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "QuestionnaireTemplate",
                keyColumn: "Id",
                keyValue: new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                column: "IsLocked",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_ActiveQuestionnaireQuestionFK",
                table: "ActiveQuestionnaireResponse",
                column: "ActiveQuestionnaireQuestionFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_CustomStudentResponseFK",
                table: "ActiveQuestionnaireResponse",
                column: "CustomStudentResponseFK",
                unique: true,
                filter: "[CustomStudentResponseFK] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_CustomTeacherResponseFK",
                table: "ActiveQuestionnaireResponse",
                column: "CustomTeacherResponseFK",
                unique: true,
                filter: "[CustomTeacherResponseFK] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireOption_ActiveQuestionnaireQuestionFK",
                table: "ActiveQuestionnaireOption",
                column: "ActiveQuestionnaireQuestionFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireQuestion_ActiveQuestionnaireFK",
                table: "ActiveQuestionnaireQuestion",
                column: "ActiveQuestionnaireFK");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaireQuestion_ActiveQuestionnaireQuestionFK",
                table: "ActiveQuestionnaireResponse",
                column: "ActiveQuestionnaireQuestionFK",
                principalTable: "ActiveQuestionnaireQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomStudentResponseFK",
                table: "ActiveQuestionnaireResponse",
                column: "CustomStudentResponseFK",
                principalTable: "CustomAnswer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomTeacherResponseFK",
                table: "ActiveQuestionnaireResponse",
                column: "CustomTeacherResponseFK",
                principalTable: "CustomAnswer",
                principalColumn: "Id");
        }
    }
}
