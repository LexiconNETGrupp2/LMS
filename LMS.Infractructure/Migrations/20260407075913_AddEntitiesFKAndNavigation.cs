using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infractructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitiesFKAndNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityTypes_TypeId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_UploaderId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Activities",
                newName: "ActivityTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_TypeId",
                table: "Activities",
                newName: "IX_Activities_ActivityTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "UploaderId",
                table: "Documents",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityTypes_ActivityTypeId",
                table: "Activities",
                column: "ActivityTypeId",
                principalTable: "ActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_UploaderId",
                table: "Documents",
                column: "UploaderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityTypes_ActivityTypeId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_UploaderId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "ActivityTypeId",
                table: "Activities",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_ActivityTypeId",
                table: "Activities",
                newName: "IX_Activities_TypeId");

            migrationBuilder.AlterColumn<string>(
                name: "UploaderId",
                table: "Documents",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityTypes_TypeId",
                table: "Activities",
                column: "TypeId",
                principalTable: "ActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_UploaderId",
                table: "Documents",
                column: "UploaderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
