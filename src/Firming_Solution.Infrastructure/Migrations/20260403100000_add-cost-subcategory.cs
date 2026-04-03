using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firming_Solution.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcostsubcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "Costs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "Costs");
        }
    }
}
