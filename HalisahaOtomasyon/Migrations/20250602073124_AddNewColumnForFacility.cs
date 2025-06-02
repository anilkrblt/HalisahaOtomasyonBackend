using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HalisahaOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnForFacility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParkingLot",
                table: "Facilities",
                newName: "HasSecurityCameras");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Facilities",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "HasFirstAid",
                table: "Facilities",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasLockerRoom",
                table: "Facilities",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasParking",
                table: "Facilities",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRefereeService",
                table: "Facilities",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_Email",
                table: "Facilities",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Facilities_Email",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "HasFirstAid",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "HasLockerRoom",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "HasParking",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "HasRefereeService",
                table: "Facilities");

            migrationBuilder.RenameColumn(
                name: "HasSecurityCameras",
                table: "Facilities",
                newName: "ParkingLot");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Facilities",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
