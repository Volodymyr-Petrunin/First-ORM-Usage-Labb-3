using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SqlLabbThree.Domain;

namespace SqlLabbThree.Data;

public partial class SchoolContext : DbContext
{
    public SchoolContext(DbContextOptions<SchoolContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__8F1EF7AE1DBB19A6");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CourseDescription)
                .HasMaxLength(200)
                .HasColumnName("course_description");
            entity.Property(e => e.CourseName)
                .HasMaxLength(100)
                .HasColumnName("course_name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C52E0BA8DE426717");

            entity.HasIndex(e => e.EmployeePersonalNumber, "UQ__Employee__A100EF3AF9AB67E8").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(30)
                .HasColumnName("employee_name");
            entity.Property(e => e.EmployeePersonalNumber)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("employee_personal_number");
            entity.Property(e => e.EmployeeSurname)
                .HasMaxLength(50)
                .HasColumnName("employee_surname");

            entity.HasMany(d => d.EcCourses).WithMany(p => p.EcTeachers)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeesCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("EcCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Employees__ec_co__6383C8BA"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("EcTeacherId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Employees__ec_te__628FA481"),
                    j =>
                    {
                        j.HasKey("EcTeacherId", "EcCourseId").HasName("PK__Employee__7BCE67CCEF0DC4DC");
                        j.ToTable("Employees_Course");
                        j.IndexerProperty<int>("EcTeacherId").HasColumnName("ec_teacher_id");
                        j.IndexerProperty<int>("EcCourseId").HasColumnName("ec_course_id");
                    });

            entity.HasMany(d => d.ErRoles).WithMany(p => p.ErEmployees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("ErRoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_er_role_id"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("ErEmployeeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_er_employee_id"),
                    j =>
                    {
                        j.HasKey("ErEmployeeId", "ErRoleId");
                        j.ToTable("Employee_Role");
                        j.IndexerProperty<int>("ErEmployeeId").HasColumnName("er_employee_id");
                        j.IndexerProperty<int>("ErRoleId").HasColumnName("er_role_id");
                    });
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__3A8F732C74B59BDE");

            entity.Property(e => e.GradeId).HasColumnName("grade_id");
            entity.Property(e => e.GradeCourse).HasColumnName("grade_course");
            entity.Property(e => e.GradeDate).HasColumnName("grade_date");
            entity.Property(e => e.GradeMark)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("grade_mark");
            entity.Property(e => e.GradeStudent).HasColumnName("grade_student");
            entity.Property(e => e.GradeTeacher).HasColumnName("grade_teacher");

            entity.HasOne(d => d.GradeCourseNavigation).WithMany(p => p.Grades)
                .HasForeignKey(d => d.GradeCourse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__grade_co__66603565");

            entity.HasOne(d => d.GradeStudentNavigation).WithMany(p => p.Grades)
                .HasForeignKey(d => d.GradeStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__grade_st__68487DD7");

            entity.HasOne(d => d.GradeTeacherNavigation).WithMany(p => p.Grades)
                .HasForeignKey(d => d.GradeTeacher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__grade_te__6754599E");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__D57795A0F5F36BC5");

            entity.HasIndex(e => e.GroupName, "UQ__Groups__E8F4F58D3F18FAD0").IsUnique();

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupMentor).HasColumnName("group_mentor");
            entity.Property(e => e.GroupName)
                .HasMaxLength(100)
                .HasColumnName("group_name");

            entity.HasOne(d => d.GroupMentorNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.GroupMentor)
                .HasConstraintName("FK__Groups__group_me__571DF1D5");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("PK__Lessons__6421F7BE9E889924");

            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.LessonAudience)
                .HasMaxLength(50)
                .HasColumnName("lesson_audience");
            entity.Property(e => e.LessonCourseId).HasColumnName("lesson_course_id");
            entity.Property(e => e.LessonDayOfWeek)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("lesson_day_of_week");
            entity.Property(e => e.LessonEndTime).HasColumnName("lesson_end_time");
            entity.Property(e => e.LessonGroupId).HasColumnName("lesson_group_id");
            entity.Property(e => e.LessonName)
                .HasMaxLength(100)
                .HasColumnName("lesson_name");
            entity.Property(e => e.LessonStartTime).HasColumnName("lesson_start_time");
            entity.Property(e => e.LessonTeacherId).HasColumnName("lesson_teacher_id");

            entity.HasOne(d => d.LessonCourse).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.LessonCourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lessons__lesson___5AEE82B9");

            entity.HasOne(d => d.LessonGroup).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.LessonGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lessons__lesson___5BE2A6F2");

            entity.HasOne(d => d.LessonTeacher).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.LessonTeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lessons__lesson___59FA5E80");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC7554AA8F");

            entity.HasIndex(e => e.RoleTitle, "UQ__Roles__B000D98B391C53B3").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleTitle)
                .HasMaxLength(30)
                .HasColumnName("role_title");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__2A33069A065CE759");

            entity.HasIndex(e => e.StudentPersonalNumber, "UQ__Students__5F7ED229D3AD60B2").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.StudentGroupId).HasColumnName("student_group_id");
            entity.Property(e => e.StudentName)
                .HasMaxLength(30)
                .HasColumnName("student_name");
            entity.Property(e => e.StudentPersonalNumber)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("student_personal_number");
            entity.Property(e => e.StudentSurname)
                .HasMaxLength(50)
                .HasColumnName("student_surname");

            entity.HasOne(d => d.StudentGroup).WithMany(p => p.Students)
                .HasForeignKey(d => d.StudentGroupId)
                .HasConstraintName("FK__Students__studen__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
