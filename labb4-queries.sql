-- 1. Show employees, their roles and completed years of service as of today or their departure date.
--    Years are counted up to today, or up to the departure date for employees who have left.
SELECT
    e.employee_id,
    e.employee_name,
    e.employee_surname,
    r.role_title,
    e.employee_start_day,
    e.employee_end_day,
    years.completed AS years_at_school
FROM Employees e
         CROSS APPLY (SELECT CAST(GETDATE() AS date) AS today) AS clock
         CROSS APPLY (SELECT IIF(e.employee_end_day < clock.today, e.employee_end_day, clock.today) AS end_date) AS employment
    -- DATEDIFF(year) counts calendar-year boundaries, not full years:
    -- subtract 1 if the anniversary in the last year has not been reached yet.
         CROSS APPLY (SELECT DATEDIFF(year, e.employee_start_day, employment.end_date) AS boundaries) AS diff
         CROSS APPLY (SELECT CASE
                                 WHEN e.employee_start_day > employment.end_date THEN 0
                                 WHEN DATEADD(year, diff.boundaries, e.employee_start_day) > employment.end_date
                                     THEN diff.boundaries - 1
                                 ELSE diff.boundaries
                                 END AS completed) AS years
         LEFT JOIN Employee_Role er ON er.er_employee_id = e.employee_id
         LEFT JOIN Roles r          ON r.role_id = er.er_role_id
ORDER BY e.employee_surname, e.employee_name, r.role_title;

GO

-- 2. Add an employee using the procedure in labb4-objects.sql; check that the personal number is unused.
-- EXEC dbo.add_employee @Name = N'Anna', @Surname = N'Example',
--     @PersonalNumber = '19900101-0017', @DepartmentName = N'Science',
--     @Salary = 40000.00, @RoleTitle = N'Teacher';

-- Duplicate test: this number belongs to Johan Lund when seed-data.sql has been run.
-- EXEC dbo.add_employee @Name = N'Anna', @Surname = N'Example',
--     @PersonalNumber = '19810512-9111', @DepartmentName = N'Science',
--     @Salary = 40000.00, @RoleTitle = N'Teacher';

GO

-- 3. Show Students with courses marks and teachers
SELECT s.student_id, s.student_name, s.student_surname, g.group_name,
       c.course_name, gr.grade_mark, gr.grade_date,
       t.employee_name AS teacher_name, t.employee_surname AS teacher_surname
            FROM Students s
                LEFT JOIN Groups g ON g.group_id = s.student_group_id
                LEFT JOIN Grades gr ON gr.grade_student = s.student_id
                LEFT JOIN Courses c ON c.course_id = gr.grade_course
                LEFT JOIN Employees t ON t.employee_id = gr.grade_teacher
                    ORDER BY s.student_surname, s.student_name, gr.grade_date;
GO

-- 4–5. Show the total and average monthly salary of currently active employees in each department.
SELECT d.department_name,
       COUNT(e.employee_id) AS employee_count,
       CAST(COALESCE(SUM(e.employee_salary), 0) AS DECIMAL(10,2)) AS monthly_salary_total,
       CAST(AVG(e.employee_salary) AS DECIMAL(10,2)) AS average_monthly_salary
            FROM Departments d
                LEFT JOIN Employees e ON e.department_id = d.department_id
                    AND e.employee_start_day <= CAST(GETDATE() AS date)
                    AND (e.employee_end_day IS NULL OR e.employee_end_day >= CAST(GETDATE() AS date))
                        GROUP BY d.department_id, d.department_name
                        ORDER BY d.department_name;
GO

-- 6. Show a student card by ID (returns an empty result if the ID does not exist).
EXEC dbo.get_student_by_id @StudentId = 1;

-- 7. Add a grade using the procedure in labb4-objects.sql; replace the IDs before running.
-- EXEC dbo.add_grade @StudentId = 1, @CourseId = 1, @TeacherId = 1, @Mark = 'A';
GO

-- Extra 1. Show a student, their class, its lesson teachers, and grades with the teachers who gave them.
DECLARE @StudentId int = 1;
SELECT s.student_id, s.student_name, s.student_surname, g.group_name,
       teachers.teacher_names, c.course_name, gr.grade_mark, gr.grade_date,
       t.employee_name AS grading_teacher_name, t.employee_surname AS grading_teacher_surname
FROM Students s
LEFT JOIN Groups g ON g.group_id = s.student_group_id
OUTER APPLY (
    SELECT STRING_AGG(CONVERT(nvarchar(max), names.teacher_name), N', ') AS teacher_names
    FROM (
        SELECT DISTINCT e.employee_id, CONCAT(e.employee_name, N' ', e.employee_surname) AS teacher_name
        FROM Lessons l
        JOIN Employees e ON e.employee_id = l.lesson_teacher_id
        WHERE l.lesson_group_id = s.student_group_id
    ) names
) teachers
LEFT JOIN Grades gr ON gr.grade_student = s.student_id
LEFT JOIN Courses c ON c.course_id = gr.grade_course
LEFT JOIN Employees t ON t.employee_id = gr.grade_teacher
WHERE s.student_id = @StudentId
ORDER BY gr.grade_date, gr.grade_id;

GO

-- Extra 2. Show teachers and their associated courses from the view in labb4-objects.sql.
SELECT * FROM dbo.teacher_courses ORDER BY teacher_surname, teacher_name, course_name;