using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HalisahaOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class newAddNewColumnsAndBugFixesss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "TeamMembers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_CustomerId",
                table: "TeamMembers",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_AspNetUsers_CustomerId",
                table: "TeamMembers",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_AspNetUsers_CustomerId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_CustomerId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "TeamMembers");
        }
    }
}
