using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlLabbThree.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentsAndEmploymentDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "employee_end_day",
                table: "Employees",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "employee_salary",
                table: "Employees",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "employee_start_day",
                table: "Employees",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "course_end_day",
                table: "Courses",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "course_start_day",
                table: "Courses",
                type: "date",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    department_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    department_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.department_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_department_id",
                table: "Employees",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_department_name",
                table: "Departments",
                column: "department_name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_department_id",
                table: "Employees",
                column: "department_id",
                principalTable: "Departments",
                principalColumn: "department_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_department_id",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Employees_department_id",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "employee_end_day",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "employee_salary",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "employee_start_day",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "course_end_day",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "course_start_day",
                table: "Courses");
        }
    }
}
