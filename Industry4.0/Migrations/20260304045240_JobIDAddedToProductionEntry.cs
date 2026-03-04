using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Industry4._0.Migrations
{
    /// <inheritdoc />
    public partial class JobIDAddedToProductionEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "ProductionEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobId",
                table: "ProductionEntries");
        }
    }
}
