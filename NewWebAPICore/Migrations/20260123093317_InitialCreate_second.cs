using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewWebAPICore.Migrations
{
    public partial class InitialCreate_second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Productions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Productions_UserId",
                table: "Productions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productions_Users_UserId",
                table: "Productions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productions_Users_UserId",
                table: "Productions");

            migrationBuilder.DropIndex(
                name: "IX_Productions_UserId",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Productions");
        }
    }
}
