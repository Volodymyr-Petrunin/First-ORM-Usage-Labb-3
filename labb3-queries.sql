SELECT employee.employee_id, employee.employee_name, employee.employee_surname, employee.employee_personal_number, role.role_title
    FROM Employees employee
         JOIN Employee_Role er ON employee.employee_id = er.er_employee_id
         JOIN Roles role ON er.er_role_id = role.role_id;

SELECT employee.employee_id, employee.employee_name, employee.employee_surname, employee.employee_personal_number, role.role_title
FROM Employees employee
         JOIN Employee_Role er ON employee.employee_id = er.er_employee_id
         JOIN Roles role ON er.er_role_id = role.role_id
            WHERE role.role_title = 'Teacher';

GO

SELECT student_name, student_surname, course_name, grade_mark, grade_date
    FROM Students student
        LEFT JOIN Grades grade ON student.student_id = grade.grade_student
        LEFT JOIN Courses cours ON cours.course_id = grade.grade_course
            WHERE grade.grade_date >= DATETRUNC(month, GETDATE())
                AND grade.grade_date < DATEADD(month, 1, DATETRUNC(month, GETDATE()))
                    ORDER BY grade.grade_date;

GO


SELECT
    cours.course_name,
    COUNT(metric.merit_score) AS grades_count,
    ROUND(AVG(metric.merit_score), 2) AS avg_merit,
    MAX(metric.merit_score) AS max_merit,
    MIN(metric.merit_score) AS min_merit
        FROM Courses cours
            JOIN Grades grade ON cours.course_id = grade.grade_course
                CROSS APPLY (
                    SELECT CASE grade.grade_mark
                        WHEN 'A' THEN 20.0
                        WHEN 'B' THEN 17.5
                        WHEN 'C' THEN 15.0
                        WHEN 'D' THEN 12.5
                        WHEN 'E' THEN 10.0
                        WHEN 'F' THEN 0.0
                        END AS merit_score
                ) metric
                    GROUP BY cours.course_id, cours.course_name
                        ORDER BY avg_merit DESC;

GO



CREATE OR ALTER FUNCTION is_valid_personal_number(@PersonalNumber VARCHAR(50)) RETURNS BIT
    AS 
    BEGIN 
        IF @PersonalNumber IS NULL
            OR @PersonalNumber NOT LIKE '[12][0-9][0-9][0-9][01][0-9][0-3][0-9]-[0-9][0-9][0-9][0-9]'
                RETURN 0;

        IF TRY_CAST(LEFT(@PersonalNumber, 8) AS DATE) IS NULL
            RETURN 0;

        DECLARE @Digits CHAR(10) = RIGHT(REPLACE(@PersonalNumber, '-', ''), 10);
        DECLARE @Sum INT = 0;
        DECLARE @Position INT = 1;
        DECLARE @Digit INT;
        
        WHILE @Position <= 10
            BEGIN
                SET @Digit = CAST(SUBSTRING(@Digits, @Position, 1) AS INT);

                IF @Position % 2 = 1
                    BEGIN
                        SET @Digit = @Digit * 2;
                        IF @Digit > 9
                            SET @Digit = @Digit - 9;
                    END

                SET @Sum = @Sum + @Digit;
                SET @Position = @Position + 1;
            END

        RETURN CASE WHEN @Sum % 10 = 0 THEN 1 ELSE 0 END;
    END

GO


UPDATE Students SET student_personal_number = '20120314-1237' WHERE student_id = 1;
UPDATE Students SET student_personal_number = '20110822-2348' WHERE student_id = 2;
UPDATE Students SET student_personal_number = '20120105-3459' WHERE student_id = 3;
UPDATE Students SET student_personal_number = '20130211-4564' WHERE student_id = 4;
UPDATE Students SET student_personal_number = '20120719-5676' WHERE student_id = 5;

GO


ALTER TABLE Students ADD CONSTRAINT CK_Students_PersonalNumber
    CHECK (dbo.is_valid_personal_number(student_personal_number) = 1);

GO

-- Failed Insert--
INSERT INTO Students (student_name, student_surname, student_personal_number, student_group_id)
    VALUES (N'Test', N'Invalid', '19810512-9112', NULL);

-- Success Insert--
INSERT INTO Students (student_name, student_surname, student_personal_number, student_group_id)
    VALUES (N'Test', N'Valid', '19810512-9111', 1);