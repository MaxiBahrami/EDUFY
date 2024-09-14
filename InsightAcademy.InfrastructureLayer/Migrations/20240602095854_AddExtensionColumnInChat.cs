using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightAcademy.InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddExtensionColumnInChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Chat",
                newName: "Size");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Chat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "File",
                table: "Chat",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "File",
                table: "Chat");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Chat",
                newName: "Url");
        }
    }
}
