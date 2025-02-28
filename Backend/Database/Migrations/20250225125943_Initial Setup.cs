using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LogLevel = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomAnswer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    LastUpated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrimaryRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireTemplateQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prompt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AllowCustom = table.Column<bool>(type: "bit", nullable: false),
                    QuestionnaireTemplateFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireTemplateQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireTemplateQuestion_QuestionnaireTemplate_QuestionnaireTemplateFK",
                        column: x => x.QuestionnaireTemplateFK,
                        principalTable: "QuestionnaireTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveQuestionnaire",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentFK = table.Column<int>(type: "int", nullable: false),
                    TeacherFK = table.Column<int>(type: "int", nullable: false),
                    QuestionnaireTemplateFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    StudentCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TeacherCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_QuestionnaireTemplate_QuestionnaireTemplateFK",
                        column: x => x.QuestionnaireTemplateFK,
                        principalTable: "QuestionnaireTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_User_StudentFK",
                        column: x => x.StudentFK,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_User_TeacherFK",
                        column: x => x.TeacherFK,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrackedRefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<byte[]>(type: "varbinary(900)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    UserFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedRefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedRefreshToken_User_UserFK",
                        column: x => x.UserFK,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireTemplateOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionValue = table.Column<int>(type: "int", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    QuestionFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireTemplateOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireTemplateOption_QuestionnaireTemplateQuestion_QuestionFK",
                        column: x => x.QuestionFK,
                        principalTable: "QuestionnaireTemplateQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveQuestionnaireQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActiveQuestionnaireFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "ActiveQuestionnaireOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionValue = table.Column<int>(type: "int", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActiveQuestionnaireQuestionFK = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ActiveQuestionnaireResponse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActiveQuestionnaireQuestionFK = table.Column<int>(type: "int", nullable: false),
                    ActiveQuestionnaireFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeacherResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomStudentResponseFK = table.Column<int>(type: "int", nullable: true),
                    CustomTeacherResponseFK = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaireResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaireQuestion_ActiveQuestionnaireQuestionFK",
                        column: x => x.ActiveQuestionnaireQuestionFK,
                        principalTable: "ActiveQuestionnaireQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaire_ActiveQuestionnaireFK",
                        column: x => x.ActiveQuestionnaireFK,
                        principalTable: "ActiveQuestionnaire",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomStudentResponseFK",
                        column: x => x.CustomStudentResponseFK,
                        principalTable: "CustomAnswer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomTeacherResponseFK",
                        column: x => x.CustomTeacherResponseFK,
                        principalTable: "CustomAnswer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_QuestionnaireTemplateFK",
                table: "ActiveQuestionnaire",
                column: "QuestionnaireTemplateFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_StudentFK",
                table: "ActiveQuestionnaire",
                column: "StudentFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_TeacherFK",
                table: "ActiveQuestionnaire",
                column: "TeacherFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_Title",
                table: "ActiveQuestionnaire",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireOption_ActiveQuestionnaireQuestionFK",
                table: "ActiveQuestionnaireOption",
                column: "ActiveQuestionnaireQuestionFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireQuestion_ActiveQuestionnaireFK",
                table: "ActiveQuestionnaireQuestion",
                column: "ActiveQuestionnaireFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_ActiveQuestionnaireFK",
                table: "ActiveQuestionnaireResponse",
                column: "ActiveQuestionnaireFK");

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
                name: "IX_QuestionnaireTemplate_CreatedAt_Id",
                table: "QuestionnaireTemplate",
                columns: new[] { "CreatedAt", "Id" },
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplate_TemplateTitle",
                table: "QuestionnaireTemplate",
                column: "TemplateTitle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplate_TemplateTitle_Id",
                table: "QuestionnaireTemplate",
                columns: new[] { "TemplateTitle", "Id" },
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplateOption_QuestionFK",
                table: "QuestionnaireTemplateOption",
                column: "QuestionFK");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplateQuestion_QuestionnaireTemplateFK",
                table: "QuestionnaireTemplateQuestion",
                column: "QuestionnaireTemplateFK");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRefreshToken_Token",
                table: "TrackedRefreshToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRefreshToken_UserFK",
                table: "TrackedRefreshToken",
                column: "UserFK");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "User",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveQuestionnaireOption");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaireResponse");

            migrationBuilder.DropTable(
                name: "ApplicationLogs");

            migrationBuilder.DropTable(
                name: "QuestionnaireTemplateOption");

            migrationBuilder.DropTable(
                name: "TrackedRefreshToken");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaireQuestion");

            migrationBuilder.DropTable(
                name: "CustomAnswer");

            migrationBuilder.DropTable(
                name: "QuestionnaireTemplateQuestion");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaire");

            migrationBuilder.DropTable(
                name: "QuestionnaireTemplate");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
