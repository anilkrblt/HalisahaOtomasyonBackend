using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HalisahaOtomasyon.Migrations
{
    /// <inheritdoc />
    public partial class facilityUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "Photos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_FacilityId",
                table: "Photos",
                column: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Facilities_FacilityId",
                table: "Photos",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Facilities_FacilityId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_FacilityId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Photos");
        }
    }
}
