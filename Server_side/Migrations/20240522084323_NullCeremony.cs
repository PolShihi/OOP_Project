using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server_side.Migrations
{
    /// <inheritdoc />
    public partial class NullCeremony : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Ceremonies_CeremonyId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Ceremonies_CeremonyId",
                table: "Orders",
                column: "CeremonyId",
                principalTable: "Ceremonies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Ceremonies_CeremonyId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Ceremonies_CeremonyId",
                table: "Orders",
                column: "CeremonyId",
                principalTable: "Ceremonies",
                principalColumn: "Id");
        }
    }
}
