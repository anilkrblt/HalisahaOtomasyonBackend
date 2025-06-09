using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HalisahaOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class newBugfixesEylemBoylams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasPaid",
                table: "RoomParticipants",
                newName: "IsReady");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "RoomParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasScoreBoard",
                table: "Fields",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Facilities",
                type: "decimal(18,15)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Facilities",
                type: "decimal(18,15)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipants_RoomId_IsHome",
                table: "RoomParticipants",
                columns: new[] { "RoomId", "IsHome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomParticipants_RoomId_IsHome",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "HasScoreBoard",
                table: "Fields");

            migrationBuilder.RenameColumn(
                name: "IsReady",
                table: "RoomParticipants",
                newName: "HasPaid");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Facilities",
                type: "decimal(9,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,15)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Facilities",
                type: "decimal(9,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,15)",
                oldNullable: true);
        }
    }
}
