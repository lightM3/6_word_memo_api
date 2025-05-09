using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordMemoAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAudioPathToWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioPath",
                table: "Words",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioPath",
                table: "Words");
        }
    }
}
