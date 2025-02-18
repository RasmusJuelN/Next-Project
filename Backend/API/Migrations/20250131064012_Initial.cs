using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LogLevel = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    LastUpated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RevokedRefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<byte[]>(type: "varbinary(900)", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevokedRefreshToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PrimaryRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false)
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
                    QuestionnaireTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireTemplateQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireTemplateQuestion_QuestionnaireTemplate_QuestionnaireTemplateId",
                        column: x => x.QuestionnaireTemplateId,
                        principalTable: "QuestionnaireTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveQuestionnaire",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    StudentCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TeacherCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_QuestionnaireTemplate_QuestionnaireTemplateId",
                        column: x => x.QuestionnaireTemplateId,
                        principalTable: "QuestionnaireTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_User_StudentId",
                        column: x => x.StudentId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_User_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireTemplateOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionValue = table.Column<int>(type: "int", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireTemplateOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireTemplateOption_QuestionnaireTemplateQuestion_QuestionId",
                        column: x => x.QuestionId,
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
                    Prompt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActiveQuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaireQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireQuestion_ActiveQuestionnaire_ActiveQuestionnaireId",
                        column: x => x.ActiveQuestionnaireId,
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
                    DisplayText = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ActiveQuestionnaireQuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaireOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireOption_ActiveQuestionnaireQuestion_ActiveQuestionnaireQuestionId",
                        column: x => x.ActiveQuestionnaireQuestionId,
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
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    ActiveQuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeacherResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomStudentResponseId = table.Column<int>(type: "int", nullable: false),
                    CustomTeacherResponseId = table.Column<int>(type: "int", nullable: false),
                    ActiveQuestionnaireQuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaireResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaireQuestion_ActiveQuestionnaireQuestionId",
                        column: x => x.ActiveQuestionnaireQuestionId,
                        principalTable: "ActiveQuestionnaireQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaire_ActiveQuestionnaireId",
                        column: x => x.ActiveQuestionnaireId,
                        principalTable: "ActiveQuestionnaire",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Response = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActiveQuestionnaireResponseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomAnswer_ActiveQuestionnaireResponse_ActiveQuestionnaireResponseId",
                        column: x => x.ActiveQuestionnaireResponseId,
                        principalTable: "ActiveQuestionnaireResponse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_QuestionnaireTemplateId",
                table: "ActiveQuestionnaire",
                column: "QuestionnaireTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_StudentId",
                table: "ActiveQuestionnaire",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_TeacherId",
                table: "ActiveQuestionnaire",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_Title",
                table: "ActiveQuestionnaire",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireOption_ActiveQuestionnaireQuestionId",
                table: "ActiveQuestionnaireOption",
                column: "ActiveQuestionnaireQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireQuestion_ActiveQuestionnaireId",
                table: "ActiveQuestionnaireQuestion",
                column: "ActiveQuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_ActiveQuestionnaireId",
                table: "ActiveQuestionnaireResponse",
                column: "ActiveQuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_ActiveQuestionnaireQuestionId",
                table: "ActiveQuestionnaireResponse",
                column: "ActiveQuestionnaireQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_CustomStudentResponseId",
                table: "ActiveQuestionnaireResponse",
                column: "CustomStudentResponseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_CustomTeacherResponseId",
                table: "ActiveQuestionnaireResponse",
                column: "CustomTeacherResponseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomAnswer_ActiveQuestionnaireResponseId",
                table: "CustomAnswer",
                column: "ActiveQuestionnaireResponseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplate_TemplateTitle",
                table: "QuestionnaireTemplate",
                column: "TemplateTitle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplateOption_QuestionId",
                table: "QuestionnaireTemplateOption",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplateQuestion_QuestionnaireTemplateId",
                table: "QuestionnaireTemplateQuestion",
                column: "QuestionnaireTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_RevokedRefreshToken_Token",
                table: "RevokedRefreshToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "User",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomStudentResponseId",
                table: "ActiveQuestionnaireResponse",
                column: "CustomStudentResponseId",
                principalTable: "CustomAnswer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomTeacherResponseId",
                table: "ActiveQuestionnaireResponse",
                column: "CustomTeacherResponseId",
                principalTable: "CustomAnswer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaire_QuestionnaireTemplate_QuestionnaireTemplateId",
                table: "ActiveQuestionnaire");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaire_User_Id",
                table: "ActiveQuestionnaire");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaire_User_StudentId",
                table: "ActiveQuestionnaire");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaire_User_TeacherId",
                table: "ActiveQuestionnaire");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaireQuestion_ActiveQuestionnaireQuestionId",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaire_ActiveQuestionnaireId",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomStudentResponseId",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaireResponse_CustomAnswer_CustomTeacherResponseId",
                table: "ActiveQuestionnaireResponse");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaireOption");

            migrationBuilder.DropTable(
                name: "ApplicationLogs");

            migrationBuilder.DropTable(
                name: "QuestionnaireTemplateOption");

            migrationBuilder.DropTable(
                name: "RevokedRefreshToken");

            migrationBuilder.DropTable(
                name: "QuestionnaireTemplateQuestion");

            migrationBuilder.DropTable(
                name: "QuestionnaireTemplate");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaireQuestion");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaire");

            migrationBuilder.DropTable(
                name: "CustomAnswer");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaireResponse");
        }
    }
}
