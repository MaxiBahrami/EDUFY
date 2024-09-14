using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightAcademy.InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageTypesInCaht : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Chat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Chat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Chat",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Chat");
        }
    }
}
