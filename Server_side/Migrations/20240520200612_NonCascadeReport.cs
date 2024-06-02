using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server_side.Migrations
{
    /// <inheritdoc />
    public partial class NonCascadeReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_AppUserId",
                table: "Reports");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_AppUserId",
                table: "Reports",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_AppUserId",
                table: "Reports");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_AppUserId",
                table: "Reports",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
