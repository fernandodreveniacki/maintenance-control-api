using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaintenanceControlSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Alerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_CreatedByUserId",
                table: "Alerts",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Users_CreatedByUserId",
                table: "Alerts",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Users_CreatedByUserId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_CreatedByUserId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Alerts");
        }
    }
}
