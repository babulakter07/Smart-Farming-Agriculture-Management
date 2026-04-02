using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firming_Solution.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatelandentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SaleUnitPrice",
                table: "CropSeasons",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YieldUnit",
                table: "CropSeasons",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaleUnitPrice",
                table: "CropSeasons");

            migrationBuilder.DropColumn(
                name: "YieldUnit",
                table: "CropSeasons");
        }
    }
}
