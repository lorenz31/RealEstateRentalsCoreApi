using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstateCore.Infrastructure.Migrations
{
    public partial class RemovedLatLongProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "RealEstateProperties");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "RealEstateProperties");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "RealEstateProperties",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "RealEstateProperties",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
