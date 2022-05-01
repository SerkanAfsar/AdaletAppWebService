using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdaletApp.DAL.Migrations
{
    public partial class PictureSeoUrlAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureBig",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "PictureMedium",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "PictureSmall",
                table: "Articles",
                newName: "PictureUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PictureUrl",
                table: "Articles",
                newName: "PictureSmall");

            migrationBuilder.AddColumn<string>(
                name: "PictureBig",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureMedium",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
