using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstateCore.Infrastructure.Migrations
{
    public partial class ChangeLatLongDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "RealEstateProperties",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "RealEstateProperties",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "RealEstateProperties",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "RealEstateProperties",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
