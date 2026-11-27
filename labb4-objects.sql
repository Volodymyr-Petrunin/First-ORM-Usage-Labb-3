-- 2. Add an employee and assign a role in one transaction.
CREATE OR ALTER PROCEDURE dbo.add_employee
    @Name nvarchar(30),
    @Surname nvarchar(50),
    @PersonalNumber varchar(13),
    @DepartmentName nvarchar(50),
    @Salary decimal(10, 2),
    @RoleTitle nvarchar(30),
    @StartDate date = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @DepartmentId int = (SELECT department_id FROM Departments WHERE department_name = @DepartmentName);
        DECLARE @RoleId int = (SELECT role_id FROM Roles WHERE role_title = @RoleTitle);
        IF @DepartmentId IS NULL
            THROW 50001, 'Department not found', 1;
        IF @RoleId IS NULL
            THROW 50002, 'Role not found', 1;

        INSERT INTO Employees (employee_name, employee_surname, employee_personal_number,
                               department_id, employee_salary, employee_start_day)
        VALUES (@Name, @Surname, @PersonalNumber, @DepartmentId, @Salary, COALESCE(@StartDate, CAST(GETDATE() AS date)));

        INSERT INTO Employee_Role (er_employee_id, er_role_id)
        VALUES (CAST(SCOPE_IDENTITY() AS int), @RoleId);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
    
GO

-- 6. Create a procedure that shows a student and their class by ID.
CREATE OR ALTER PROCEDURE dbo.get_student_by_id @StudentId int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.student_id, s.student_name, s.student_surname, s.student_personal_number,
           g.group_id, g.group_name
    FROM Students s
             LEFT JOIN Groups g ON g.group_id = s.student_group_id
    WHERE s.student_id = @StudentId;
END;
    
GO

-- 7. Add a grade in a transaction.
CREATE OR ALTER PROCEDURE dbo.add_grade
    @StudentId int,
    @CourseId int,
    @TeacherId int,
    @Mark nvarchar(max),
    @GradeDate date = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Students WHERE student_id = @StudentId)
            THROW 50003, 'Student not found', 1;
        IF NOT EXISTS (SELECT 1 FROM Courses WHERE course_id = @CourseId)
            THROW 50004, 'Course not found', 1;
        IF NOT EXISTS (SELECT 1 FROM Employees WHERE employee_id = @TeacherId)
            THROW 50005, 'Teacher not found', 1;
        IF NOT EXISTS (SELECT 1 FROM Employees_Course WITH (HOLDLOCK)
                       WHERE ec_teacher_id = @TeacherId AND ec_course_id = @CourseId)
            THROW 50006, 'Teacher does not teach this course', 1;

        SET @Mark = UPPER(@Mark);
        -- DATALENGTH, not LEN: LEN ignores trailing spaces ('A ' would pass); nvarchar = 2 bytes per char
        IF @Mark IS NULL OR DATALENGTH(@Mark) <> 2
            OR @Mark NOT IN (N'A', N'B', N'C', N'D', N'E', N'F')
            THROW 50007, 'Grade must be a letter from A to F', 1;

        INSERT INTO Grades (grade_student, grade_course, grade_teacher, grade_mark, grade_date)
        VALUES (@StudentId, @CourseId, @TeacherId, @Mark, COALESCE(@GradeDate, CAST(GETDATE() AS date)));
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
    
GO

-- Extra 2. Create a view linking teachers to courses they can teach.
CREATE OR ALTER VIEW dbo.teacher_courses
AS
SELECT e.employee_id AS teacher_id, e.employee_name AS teacher_name,
       e.employee_surname AS teacher_surname, c.course_id, c.course_name
FROM Employees_Course ec
JOIN Employees e ON e.employee_id = ec.ec_teacher_id
JOIN Courses c ON c.course_id = ec.ec_course_id;

GO