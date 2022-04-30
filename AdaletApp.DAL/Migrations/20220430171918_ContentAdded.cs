using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdaletApp.DAL.Migrations
{
    public partial class ContentAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewsContent",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewsContent",
                table: "Articles");
        }
    }
}
