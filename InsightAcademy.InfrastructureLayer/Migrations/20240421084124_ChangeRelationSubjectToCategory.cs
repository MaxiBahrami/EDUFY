using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightAcademy.InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationSubjectToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorSubject_Subject_SubjectId",
                table: "TutorSubject");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "TutorSubject",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_TutorSubject_SubjectId",
                table: "TutorSubject",
                newName: "IX_TutorSubject_CategoryId");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "TutorSubject",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "TutorSubject");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "TutorSubject",
                newName: "SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TutorSubject_CategoryId",
                table: "TutorSubject",
                newName: "IX_TutorSubject_SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorSubject_Subject_SubjectId",
                table: "TutorSubject",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
