using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class TrackuserbyGuidinsteadofuserinDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackedRefreshToken_User_UserFK",
                table: "TrackedRefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_TrackedRefreshToken_UserFK",
                table: "TrackedRefreshToken");

            migrationBuilder.DropColumn(
                name: "UserFK",
                table: "TrackedRefreshToken");

            migrationBuilder.AddColumn<int>(
                name: "UserBaseModelId",
                table: "TrackedRefreshToken",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserGuid",
                table: "TrackedRefreshToken",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRefreshToken_UserBaseModelId",
                table: "TrackedRefreshToken",
                column: "UserBaseModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackedRefreshToken_User_UserBaseModelId",
                table: "TrackedRefreshToken",
                column: "UserBaseModelId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackedRefreshToken_User_UserBaseModelId",
                table: "TrackedRefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_TrackedRefreshToken_UserBaseModelId",
                table: "TrackedRefreshToken");

            migrationBuilder.DropColumn(
                name: "UserBaseModelId",
                table: "TrackedRefreshToken");

            migrationBuilder.DropColumn(
                name: "UserGuid",
                table: "TrackedRefreshToken");

            migrationBuilder.AddColumn<int>(
                name: "UserFK",
                table: "TrackedRefreshToken",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRefreshToken_UserFK",
                table: "TrackedRefreshToken",
                column: "UserFK");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackedRefreshToken_User_UserFK",
                table: "TrackedRefreshToken",
                column: "UserFK",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
