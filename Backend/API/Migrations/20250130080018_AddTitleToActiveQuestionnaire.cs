using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleToActiveQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ActiveQuestionnaire",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_Title",
                table: "ActiveQuestionnaire",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActiveQuestionnaire_Title",
                table: "ActiveQuestionnaire");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ActiveQuestionnaire");
        }
    }
}
