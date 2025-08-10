using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FishSupplyChain.Migrations
{
    /// <inheritdoc />
    public partial class UserLinkedToFarm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Farms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Farms_UserId",
                table: "Farms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Farms_Users_UserId",
                table: "Farms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Farms_Users_UserId",
                table: "Farms");

            migrationBuilder.DropIndex(
                name: "IX_Farms_UserId",
                table: "Farms");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Farms");
        }
    }
}
