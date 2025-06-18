using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HalisahaOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsHomeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomParticipants_RoomId_IsHome",
                table: "RoomParticipants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipants_RoomId_IsHome",
                table: "RoomParticipants",
                columns: new[] { "RoomId", "IsHome" },
                unique: true);
        }
    }
}
