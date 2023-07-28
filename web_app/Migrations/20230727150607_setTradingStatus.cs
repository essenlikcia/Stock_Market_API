using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_app.Migrations
{
    public partial class setTradingStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isTradingAllowed",
                table: "Stocks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isTradingAllowed",
                table: "Stocks");
        }
    }
}
