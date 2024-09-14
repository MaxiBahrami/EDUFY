using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightAcademy.InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColoumnsInTutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlock",
                table: "Tutor",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlock",
                table: "Tutor");
        }
    }
}
