using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedLib.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhotoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Photos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Photos",
                type: "text",
                nullable: true);
        }
    }
}
