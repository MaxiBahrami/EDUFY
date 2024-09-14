using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightAcademy.InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryInTutorSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "TutorSubject",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TutorSubject_CategoryId",
                table: "TutorSubject",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorSubject_Category_CategoryId",
                table: "TutorSubject",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorSubject_Category_CategoryId",
                table: "TutorSubject");

            migrationBuilder.DropIndex(
                name: "IX_TutorSubject_CategoryId",
                table: "TutorSubject");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TutorSubject");
        }
    }
}
