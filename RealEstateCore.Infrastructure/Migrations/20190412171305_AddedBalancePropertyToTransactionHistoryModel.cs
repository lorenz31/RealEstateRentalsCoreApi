using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstateCore.Infrastructure.Migrations
{
    public partial class AddedBalancePropertyToTransactionHistoryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Transactions");
        }
    }
}
