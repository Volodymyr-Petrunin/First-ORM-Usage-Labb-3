using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlLabbThree.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Role_er_role_id",
                table: "Employee_Role",
                column: "er_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Course_ec_course_id",
                table: "Employees_Course",
                column: "ec_course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_grade_course",
                table: "Grades",
                column: "grade_course");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_grade_student",
                table: "Grades",
                column: "grade_student");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_grade_teacher",
                table: "Grades",
                column: "grade_teacher");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_group_mentor",
                table: "Groups",
                column: "group_mentor");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_lesson_course_id",
                table: "Lessons",
                column: "lesson_course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_lesson_group_id",
                table: "Lessons",
                column: "lesson_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_lesson_teacher_id",
                table: "Lessons",
                column: "lesson_teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_student_group_id",
                table: "Students",
                column: "student_group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropIndex(
                name: "IX_Students_student_group_id",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_lesson_teacher_id",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_lesson_group_id",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_lesson_course_id",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Groups_group_mentor",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Grades_grade_teacher",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_grade_student",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_grade_course",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Course_ec_course_id",
                table: "Employees_Course");

            migrationBuilder.DropIndex(
                name: "IX_Employee_Role_er_role_id",
                table: "Employee_Role");
        }
    }
}
