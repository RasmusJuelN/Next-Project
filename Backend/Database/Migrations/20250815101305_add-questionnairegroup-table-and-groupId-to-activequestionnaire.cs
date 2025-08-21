using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class addquestionnairegrouptableandgroupIdtoactivequestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "ActiveQuestionnaire",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionnaireGroups",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireGroups", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_QuestionnaireGroups_QuestionnaireTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "QuestionnaireTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_GroupId",
                table: "ActiveQuestionnaire",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireGroups_TemplateId",
                table: "QuestionnaireGroups",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveQuestionnaire_QuestionnaireGroups_GroupId",
                table: "ActiveQuestionnaire",
                column: "GroupId",
                principalTable: "QuestionnaireGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveQuestionnaire_QuestionnaireGroups_GroupId",
                table: "ActiveQuestionnaire");

            migrationBuilder.DropTable(
                name: "QuestionnaireGroups");

            migrationBuilder.DropIndex(
                name: "IX_ActiveQuestionnaire_GroupId",
                table: "ActiveQuestionnaire");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ActiveQuestionnaire");
        }
    }
}
