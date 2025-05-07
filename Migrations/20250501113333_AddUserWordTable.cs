using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordMemoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserWordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "NextRepetitionDate",
                table: "UserWords",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "IsMastered",
                table: "UserWords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WordId1",
                table: "UserWords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWords_WordId1",
                table: "UserWords",
                column: "WordId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWords_Words_WordId1",
                table: "UserWords",
                column: "WordId1",
                principalTable: "Words",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWords_Words_WordId1",
                table: "UserWords");

            migrationBuilder.DropIndex(
                name: "IX_UserWords_WordId1",
                table: "UserWords");

            migrationBuilder.DropColumn(
                name: "IsMastered",
                table: "UserWords");

            migrationBuilder.DropColumn(
                name: "WordId1",
                table: "UserWords");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextRepetitionDate",
                table: "UserWords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
