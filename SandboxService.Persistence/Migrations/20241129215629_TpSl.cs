using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandboxService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TpSl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "StopLoss",
                table: "MarginPositions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TakeProfit",
                table: "MarginPositions",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StopLoss",
                table: "MarginPositions");

            migrationBuilder.DropColumn(
                name: "TakeProfit",
                table: "MarginPositions");
        }
    }
}
