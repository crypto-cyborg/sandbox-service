using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandboxService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PositionExitPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExitPrice",
                table: "MarginPositions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExitPrice",
                table: "MarginPositions");
        }
    }
}
