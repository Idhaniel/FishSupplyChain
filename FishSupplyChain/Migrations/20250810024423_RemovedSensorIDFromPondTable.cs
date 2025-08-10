using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FishSupplyChain.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSensorIDFromPondTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "Ponds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SensorId",
                table: "Ponds",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
