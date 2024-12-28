using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandboxService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MinorUpdates1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Orders",
                newName: "PositionAmount");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "MarginPositions",
                newName: "PositionAmount");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Leverage",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Leverage",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PositionAmount",
                table: "Orders",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "PositionAmount",
                table: "MarginPositions",
                newName: "Amount");
        }
    }
}
