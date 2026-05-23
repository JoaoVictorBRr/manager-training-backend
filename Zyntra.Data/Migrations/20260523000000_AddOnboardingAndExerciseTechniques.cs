using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zyntra.Data.Migrations
{
    public partial class AddOnboardingAndExerciseTechniques : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Student: onboarding fields
            migrationBuilder.AddColumn<string>(
                name: "Objective",
                table: "Student",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OnboardingDataJson",
                table: "Student",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "OnboardingCompleted",
                table: "Student",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            // Exercise: change Repetitions from int to string
            migrationBuilder.AlterColumn<string>(
                name: "Repetitions",
                table: "Exercise",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            // Exercise: advanced technique fields
            migrationBuilder.AddColumn<string>(
                name: "RestTime",
                table: "Exercise",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsDropset",
                table: "Exercise",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SupersetWith",
                table: "Exercise",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "ToFailure",
                table: "Exercise",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RIR",
                table: "Exercise",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cadence",
                table: "Exercise",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AdvancedTechniques",
                table: "Exercise",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Objective", table: "Student");
            migrationBuilder.DropColumn(name: "OnboardingDataJson", table: "Student");
            migrationBuilder.DropColumn(name: "OnboardingCompleted", table: "Student");
            migrationBuilder.DropColumn(name: "RestTime", table: "Exercise");
            migrationBuilder.DropColumn(name: "IsDropset", table: "Exercise");
            migrationBuilder.DropColumn(name: "SupersetWith", table: "Exercise");
            migrationBuilder.DropColumn(name: "ToFailure", table: "Exercise");
            migrationBuilder.DropColumn(name: "RIR", table: "Exercise");
            migrationBuilder.DropColumn(name: "Cadence", table: "Exercise");
            migrationBuilder.DropColumn(name: "AdvancedTechniques", table: "Exercise");

            migrationBuilder.AlterColumn<int>(
                name: "Repetitions",
                table: "Exercise",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)");
        }
    }
}
