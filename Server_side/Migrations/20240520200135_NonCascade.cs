using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server_side.Migrations
{
    /// <inheritdoc />
    public partial class NonCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_AppUserId",
                table: "Clients");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AspNetUsers_AppUserId",
                table: "Clients",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_AppUserId",
                table: "Clients");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AspNetUsers_AppUserId",
                table: "Clients",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
