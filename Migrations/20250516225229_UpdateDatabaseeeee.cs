using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_econsulta.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseeeee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_DoctorUsers_DoctorId",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "DoctorName",
                table: "PatientUsers",
                newName: "PatientName");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Availabilities",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Availabilities",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Availabilities",
                newName: "DoctorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Availabilities_DoctorId",
                table: "Availabilities",
                newName: "IX_Availabilities_DoctorUserId");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "Availabilities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_DoctorUsers_DoctorUserId",
                table: "Availabilities",
                column: "DoctorUserId",
                principalTable: "DoctorUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_DoctorUsers_DoctorUserId",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "Availabilities");

            migrationBuilder.RenameColumn(
                name: "PatientName",
                table: "PatientUsers",
                newName: "DoctorName");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Availabilities",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Availabilities",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "DoctorUserId",
                table: "Availabilities",
                newName: "DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Availabilities_DoctorUserId",
                table: "Availabilities",
                newName: "IX_Availabilities_DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_DoctorUsers_DoctorId",
                table: "Availabilities",
                column: "DoctorId",
                principalTable: "DoctorUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
