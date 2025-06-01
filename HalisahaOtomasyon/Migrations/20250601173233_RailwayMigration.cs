using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HalisahaOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class RailwayMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId1",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId2",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId2",
                table: "Friendships");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId1_Status",
                table: "Friendships",
                columns: new[] { "UserId1", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId2_Status",
                table: "Friendships",
                columns: new[] { "UserId2", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId1",
                table: "Friendships",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId2",
                table: "Friendships",
                column: "UserId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId1",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId2",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId1_Status",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId2_Status",
                table: "Friendships");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId2",
                table: "Friendships",
                column: "UserId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId1",
                table: "Friendships",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId2",
                table: "Friendships",
                column: "UserId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
