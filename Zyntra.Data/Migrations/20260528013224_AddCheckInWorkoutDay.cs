using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zyntra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInWorkoutDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkoutDayPerformed",
                table: "CheckIn",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkoutDayPerformed",
                table: "CheckIn");
        }
    }
}
