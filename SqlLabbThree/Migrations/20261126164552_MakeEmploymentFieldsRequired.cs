using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlLabbThree.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmploymentFieldsRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "employee_start_day",
                table: "Employees",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "employee_salary",
                table: "Employees",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "department_id",
                table: "Employees",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "course_start_day",
                table: "Courses",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Employees_EndAfterStart",
                table: "Employees",
                sql: "employee_end_day IS NULL OR employee_end_day >= employee_start_day");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Employees_SalaryPositive",
                table: "Employees",
                sql: "employee_salary > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Courses_EndAfterStart",
                table: "Courses",
                sql: "course_end_day IS NULL OR course_end_day >= course_start_day");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Employees_EndAfterStart",
                table: "Employees");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Employees_SalaryPositive",
                table: "Employees");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Courses_EndAfterStart",
                table: "Courses");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "employee_start_day",
                table: "Employees",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<decimal>(
                name: "employee_salary",
                table: "Employees",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "department_id",
                table: "Employees",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "course_start_day",
                table: "Courses",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
