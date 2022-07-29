using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoleBasedIdentityAuthentication.API.Migrations
{
    public partial class Init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Integrations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_UserId",
                table: "Integrations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_AspNetUsers_UserId",
                table: "Integrations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_AspNetUsers_UserId",
                table: "Integrations");

            migrationBuilder.DropIndex(
                name: "IX_Integrations_UserId",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Integrations");
        }
    }
}
