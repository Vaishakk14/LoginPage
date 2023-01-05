using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginPage.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "LoginPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "LoginPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "LoginPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "LoginPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "LoginPage");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "LoginPage");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "LoginPage");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "LoginPage");
        }
    }
}
