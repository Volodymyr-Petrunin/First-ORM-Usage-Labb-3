SELECT employee.employee_id, employee.employee_name, employee.employee_surname, employee.employee_personal_number, role.role_title
    FROM Employees employee
        JOIN Employee_Role er ON employee.employee_id = er.er_employee_id
        JOIN Roles role ON er.er_role_id = role.role_id
            WHERE role.role_title = 'Teacher';

SELECT student_name, student_surname, course_name, grade_mark, grade_date
    FROM Students student
        JOIN Grades grade ON student.student_id = grade.grade_student
        JOIN Courses cours ON cours.course_id = grade.grade_course
            WHERE grade.grade_date >= DATETRUNC(month, GETDATE())
                AND grade.grade_date < DATEADD(month, 1, DATETRUNC(month, GETDATE()))
                    ORDER BY grade.grade_date;

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