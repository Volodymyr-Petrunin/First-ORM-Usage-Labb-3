-- Labb 3: additional seed data for the School database.
-- Runs on top of the labb 2 data. No hard-coded IDs: every foreign key is
-- resolved through a natural key (personal number, title, name).
-- Everything runs in ONE transaction: any error, or any lookup that silently
-- matches fewer rows than expected, rolls back the whole script.
--
-- Deliberate edge cases:
--   * Role 'School Nurse' still has no employees (empty category).
--   * Course 'Music' has a teacher and lessons but NO grades (LEFT JOIN test).
--   * Student Malte Blom (9A) has no grades (newly transferred).
--   * Duplicate first names (Ella, Hugo) to test the secondary sort key.
--   * Surnames with Å/Ä/Ö to test Swedish collation.
--   * Peter Sjöberg is both Teacher and Administrator.
--   * Grade dates are relative to GETDATE() at run time, spread over 100 days.
--   * All NEW personal numbers have a valid Luhn check digit;
--     the labb 2 ones (e.g. 19750412-1111) do not.

SET XACT_ABORT ON;

BEGIN TRY
BEGIN TRANSACTION;

    -- Courses
INSERT INTO Courses (course_name, course_description) VALUES
                                                          (N'Physics', N'Forces, energy and electricity'),
                                                          (N'Physical Education', N'Sports, movement and health'),
                                                          (N'Music', N'Playing, singing and music theory');
IF @@ROWCOUNT <> 3 THROW 50001, 'Courses: unexpected row count', 1;

    -- Employees
INSERT INTO Employees (employee_personal_number, employee_name, employee_surname) VALUES
                                                                                      ('19810512-9111', N'Johan', N'Lund'),
                                                                                      ('19900803-1347', N'Sofia', N'Ek'),
                                                                                      ('19870221-4134', N'Ahmed', N'Hassan'),
                                                                                      ('19931109-2705', N'Linnea', N'Dahl'),
                                                                                      ('19720630-9697', N'Peter', N'Sjöberg'),
                                                                                      ('19680914-2182', N'Ingrid', N'Åkesson'),
                                                                                      ('19770127-6417', N'Mikael', N'Strand');
IF @@ROWCOUNT <> 7 THROW 50002, 'Employees: unexpected row count', 1;

    -- Employee roles
INSERT INTO Employee_Role (er_employee_id, er_role_id)
SELECT e.employee_id, r.role_id
FROM (VALUES
          ('19810512-9111', N'Teacher'),
          ('19900803-1347', N'Teacher'),
          ('19870221-4134', N'Teacher'),
          ('19931109-2705', N'Teacher'),
          ('19720630-9697', N'Teacher'),
          ('19720630-9697', N'Administrator'),
          ('19680914-2182', N'Administrator'),
          ('19770127-6417', N'Janitor')
     ) AS src (personal_number, role_title)
         JOIN Employees e ON e.employee_personal_number = src.personal_number
         JOIN Roles r     ON r.role_title = src.role_title;
IF @@ROWCOUNT <> 8 THROW 50003, 'Employee_Role: unexpected row count', 1;

    -- Which teacher can teach which course
INSERT INTO Employees_Course (ec_teacher_id, ec_course_id)
SELECT e.employee_id, c.course_id
FROM (VALUES
          ('19810512-9111', N'Mathematics'),
          ('19810512-9111', N'Physics'),
          ('19900803-1347', N'English'),
          ('19900803-1347', N'Music'),
          ('19870221-4134', N'Physics'),
          ('19870221-4134', N'Mathematics'),
          ('19931109-2705', N'Physical Education'),
          ('19931109-2705', N'Biology'),
          ('19720630-9697', N'Swedish'),
          ('19720630-9697', N'History')
     ) AS src (personal_number, course_name)
         JOIN Employees e ON e.employee_personal_number = src.personal_number
         JOIN Courses c   ON c.course_name = src.course_name;
IF @@ROWCOUNT <> 10 THROW 50004, 'Employees_Course: unexpected row count', 1;

    -- Groups (classes)
INSERT INTO Groups (group_name, group_mentor)
SELECT src.group_name, e.employee_id
FROM (VALUES
          (N'7B', '19810512-9111'),
          (N'8A', '19900803-1347'),
          (N'9A', '19931109-2705')
     ) AS src (group_name, mentor_personal_number)
         JOIN Employees e ON e.employee_personal_number = src.mentor_personal_number;
IF @@ROWCOUNT <> 3 THROW 50005, 'Groups: unexpected row count', 1;

    -- Students
INSERT INTO Students (student_name, student_surname, student_personal_number, student_group_id)
SELECT src.student_name, src.student_surname, src.personal_number, g.group_id
FROM (VALUES
          (N'Ella', N'Karlsson', '20130103-3724', N'7A'),
          (N'Hugo', N'Persson', '20130920-1398', N'7A'),
          (N'Åsa', N'Lindgren', '20130423-9385', N'7A'),
          (N'William', N'Ängström', '20130708-6791', N'7A'),
          (N'Maja', N'Berglund', '20130526-1024', N'7A'),
          (N'Ella', N'Andersson', '20131214-5343', N'7B'),
          (N'Lucas', N'Nyberg', '20130307-5319', N'7B'),
          (N'Saga', N'Holmberg', '20130213-2244', N'7B'),
          (N'Ali', N'Rahimi', '20130620-4312', N'7B'),
          (N'Olivia', N'Sandberg', '20131215-7801', N'7B'),
          (N'Elias', N'Forsberg', '20130703-8057', N'7B'),
          (N'Hugo', N'Björk', '20121120-5693', N'8A'),
          (N'Wilma', N'Engström', '20120423-1805', N'8A'),
          (N'Adam', N'Lindholm', '20121108-4718', N'8A'),
          (N'Ebba', N'Östlund', '20120428-2261', N'8A'),
          (N'Leo', N'Mattsson', '20120515-9153', N'8A'),
          (N'Freja', N'Axelsson', '20120312-5529', N'8A'),
          (N'Alice', N'Gustafsson', '20121109-9906', N'8B'),
          (N'Viktor', N'Åberg', '20121021-3193', N'8B'),
          (N'Nora', N'Hedlund', '20121208-3065', N'8B'),
          (N'Isak', N'Söderberg', '20120709-9191', N'8B'),
          (N'Selma', N'Jonsson', '20120422-5104', N'8B'),
          (N'Astrid', N'Wikström', '20110427-1448', N'9A'),
          (N'Oliver', N'Sundberg', '20110709-1835', N'9A'),
          (N'Vera', N'Magnusson', '20111023-5023', N'9A'),
          (N'Axel', N'Eriksson', '20111116-6078', N'9A'),
          (N'Klara', N'Nordström', '20110309-2720', N'9A'),
          (N'Malte', N'Blom', '20111218-7859', N'9A'),
          (N'Tilde', N'Lundqvist', '20111219-6488', N'9C'),
          (N'Arvid', N'Falk', '20110712-3836', N'9C'),
          (N'Agnes', N'Hellström', '20110916-2105', N'9C'),
          (N'Sixten', N'Larsson', '20110205-9035', N'9C'),
          (N'Ines', N'Kovač', '20111114-8605', N'9C')
     ) AS src (student_name, student_surname, personal_number, group_name)
         JOIN Groups g ON g.group_name = src.group_name;
IF @@ROWCOUNT <> 33 THROW 50006, 'Students: unexpected row count', 1;

    -- Lessons
INSERT INTO Lessons (lesson_name, lesson_audience, lesson_day_of_week, lesson_start_time, lesson_end_time,
                     lesson_teacher_id, lesson_course_id, lesson_group_id)
SELECT src.lesson_name, src.audience, src.day_of_week, src.start_time, src.end_time,
       e.employee_id, c.course_id, g.group_id
FROM (VALUES
          (N'Mathematics 7B', N'Room 102', 'Monday', '10:00', '11:00', '19810512-9111', N'Mathematics', N'7B'),
          (N'English 7B', N'Room 203', 'Tuesday', '08:15', '09:15', '19900803-1347', N'English', N'7B'),
          (N'Physical Education 7B', N'Gym', 'Friday', '13:00', '14:00', '19931109-2705', N'Physical Education', N'7B'),
          (N'Physics 8A', N'Lab 2', 'Monday', '13:00', '14:00', '19870221-4134', N'Physics', N'8A'),
          (N'Music 8A', N'Music Room', 'Wednesday', '09:30', '10:30', '19900803-1347', N'Music', N'8A'),
          (N'Swedish 8A', N'Room 206', 'Thursday', '10:00', '11:00', '19720630-9697', N'Swedish', N'8A'),
          (N'Biology 9A', N'Lab 1', 'Tuesday', '13:00', '14:00', '19931109-2705', N'Biology', N'9A'),
          (N'History 9A', N'Room 305', 'Wednesday', '08:15', '09:15', '19720630-9697', N'History', N'9A'),
          (N'Mathematics 9A', N'Room 101', 'Friday', '08:15', '09:15', '19870221-4134', N'Mathematics', N'9A'),
          (N'Physics 9C', N'Lab 2', 'Thursday', '13:00', '14:00', '19810512-9111', N'Physics', N'9C'),
          (N'Physical Education 8B', N'Gym', 'Tuesday', '10:00', '11:00', '19931109-2705', N'Physical Education', N'8B'),
          (N'Music 7A', N'Music Room', 'Friday', '10:00', '11:00', '19900803-1347', N'Music', N'7A')
     ) AS src (lesson_name, audience, day_of_week, start_time, end_time, teacher_personal_number, course_name, group_name)
         JOIN Employees e ON e.employee_personal_number = src.teacher_personal_number
         JOIN Courses c   ON c.course_name = src.course_name
         JOIN Groups g    ON g.group_name = src.group_name;
IF @@ROWCOUNT <> 12 THROW 50007, 'Lessons: unexpected row count', 1;

    -- Grades (days_ago is relative to the moment the script runs)
INSERT INTO Grades (grade_course, grade_teacher, grade_student, grade_mark, grade_date)
SELECT c.course_id, t.employee_id, s.student_id, src.grade_mark,
       CAST(DATEADD(DAY, -src.days_ago, GETDATE()) AS DATE)
FROM (VALUES
          ('20120314-1234', N'Biology', '19931109-2705', 'E', 2),
          ('20120314-1234', N'Physical Education', '19931109-2705', 'E', 69),
          ('20120314-1234', N'History', '19720630-9697', 'F', 83),
          ('20120314-1234', N'Physics', '19870221-4134', 'B', 38),
          ('20110822-2345', N'Biology', '19931109-2705', 'D', 98),
          ('20110822-2345', N'Swedish', '19880117-3333', 'D', 14),
          ('20110822-2345', N'Physical Education', '19931109-2705', 'E', 65),
          ('20110822-2345', N'Mathematics', '19870221-4134', 'C', 20),
          ('20120105-3456', N'English', '19900803-1347', 'D', 3),
          ('20120105-3456', N'Swedish', '19880117-3333', 'D', 40),
          ('20120105-3456', N'History', '19750412-1111', 'B', 31),
          ('20120105-3456', N'Mathematics', '19870221-4134', 'B', 11),
          ('20130211-4567', N'Physics', '19810512-9111', 'E', 61),
          ('20130211-4567', N'Biology', '19820930-2222', 'C', 68),
          ('20130211-4567', N'Mathematics', '19870221-4134', 'D', 28),
          ('20130211-4567', N'Swedish', '19880117-3333', 'F', 40),
          ('20120719-5678', N'Biology', '19931109-2705', 'C', 32),
          ('20120719-5678', N'Physics', '19810512-9111', 'B', 44),
          ('20120719-5678', N'English', '19880117-3333', 'E', 71),
          ('20120719-5678', N'Physical Education', '19931109-2705', 'E', 29),
          ('20130103-3724', N'Mathematics', '19820930-2222', 'A', 43),
          ('20130103-3724', N'Physical Education', '19931109-2705', 'E', 31),
          ('20130103-3724', N'Physics', '19870221-4134', 'E', 63),
          ('20130103-3724', N'Swedish', '19880117-3333', 'E', 17),
          ('20130920-1398', N'Physics', '19810512-9111', 'D', 53),
          ('20130920-1398', N'History', '19750412-1111', 'B', 13),
          ('20130920-1398', N'Physical Education', '19931109-2705', 'D', 55),
          ('20130920-1398', N'Biology', '19931109-2705', 'D', 94),
          ('20130423-9385', N'Mathematics', '19810512-9111', 'F', 44),
          ('20130423-9385', N'Physics', '19810512-9111', 'C', 25),
          ('20130423-9385', N'Physical Education', '19931109-2705', 'E', 58),
          ('20130423-9385', N'History', '19750412-1111', 'D', 24),
          ('20130708-6791', N'English', '19900803-1347', 'E', 13),
          ('20130708-6791', N'Biology', '19820930-2222', 'E', 70),
          ('20130708-6791', N'Swedish', '19880117-3333', 'B', 97),
          ('20130708-6791', N'Mathematics', '19820930-2222', 'C', 53),
          ('20130526-1024', N'Biology', '19820930-2222', 'C', 49),
          ('20130526-1024', N'Physical Education', '19931109-2705', 'D', 34),
          ('20130526-1024', N'Swedish', '19720630-9697', 'C', 55),
          ('20130526-1024', N'Physics', '19870221-4134', 'C', 25),
          ('20131214-5343', N'English', '19900803-1347', 'B', 7),
          ('20131214-5343', N'Swedish', '19720630-9697', 'D', 68),
          ('20131214-5343', N'Mathematics', '19820930-2222', 'B', 66),
          ('20131214-5343', N'History', '19750412-1111', 'C', 9),
          ('20130307-5319', N'History', '19750412-1111', 'E', 32),
          ('20130307-5319', N'Mathematics', '19870221-4134', 'E', 6),
          ('20130307-5319', N'Swedish', '19880117-3333', 'D', 85),
          ('20130307-5319', N'Biology', '19931109-2705', 'C', 27),
          ('20130213-2244', N'Physics', '19870221-4134', 'D', 17),
          ('20130213-2244', N'Physical Education', '19931109-2705', 'D', 41),
          ('20130213-2244', N'English', '19880117-3333', 'A', 59),
          ('20130213-2244', N'Swedish', '19880117-3333', 'B', 69),
          ('20130620-4312', N'Swedish', '19720630-9697', 'B', 32),
          ('20130620-4312', N'History', '19720630-9697', 'C', 21),
          ('20130620-4312', N'English', '19900803-1347', 'E', 91),
          ('20130620-4312', N'Physical Education', '19931109-2705', 'E', 84),
          ('20131215-7801', N'History', '19750412-1111', 'C', 34),
          ('20131215-7801', N'Mathematics', '19820930-2222', 'B', 96),
          ('20131215-7801', N'Physical Education', '19931109-2705', 'C', 37),
          ('20131215-7801', N'English', '19880117-3333', 'F', 44),
          ('20130703-8057', N'Swedish', '19720630-9697', 'B', 12),
          ('20130703-8057', N'Physics', '19870221-4134', 'C', 6),
          ('20130703-8057', N'English', '19880117-3333', 'D', 99),
          ('20130703-8057', N'Biology', '19820930-2222', 'E', 34),
          ('20121120-5693', N'Swedish', '19880117-3333', 'B', 10),
          ('20121120-5693', N'Physics', '19810512-9111', 'E', 5),
          ('20121120-5693', N'Biology', '19931109-2705', 'E', 71),
          ('20121120-5693', N'History', '19750412-1111', 'D', 17),
          ('20120423-1805', N'Mathematics', '19810512-9111', 'C', 88),
          ('20120423-1805', N'English', '19880117-3333', 'E', 14),
          ('20120423-1805', N'Physics', '19870221-4134', 'F', 72),
          ('20120423-1805', N'Physical Education', '19931109-2705', 'E', 96),
          ('20121108-4718', N'Swedish', '19720630-9697', 'A', 23),
          ('20121108-4718', N'Physical Education', '19931109-2705', 'D', 86),
          ('20121108-4718', N'Physics', '19810512-9111', 'C', 21),
          ('20121108-4718', N'History', '19750412-1111', 'D', 5),
          ('20120428-2261', N'Physical Education', '19931109-2705', 'D', 40),
          ('20120428-2261', N'Biology', '19820930-2222', 'C', 4),
          ('20120428-2261', N'Swedish', '19880117-3333', 'D', 43),
          ('20120428-2261', N'History', '19720630-9697', 'B', 99),
          ('20120515-9153', N'English', '19900803-1347', 'A', 15),
          ('20120515-9153', N'Physical Education', '19931109-2705', 'C', 75),
          ('20120515-9153', N'History', '19720630-9697', 'A', 14),
          ('20120515-9153', N'Biology', '19931109-2705', 'D', 94),
          ('20120312-5529', N'Physical Education', '19931109-2705', 'E', 25),
          ('20120312-5529', N'English', '19900803-1347', 'B', 91),
          ('20120312-5529', N'Biology', '19931109-2705', 'A', 67),
          ('20120312-5529', N'Mathematics', '19870221-4134', 'E', 93),
          ('20121109-9906', N'Physics', '19870221-4134', 'B', 86),
          ('20121109-9906', N'Physical Education', '19931109-2705', 'E', 41),
          ('20121109-9906', N'Swedish', '19880117-3333', 'F', 39),
          ('20121109-9906', N'English', '19900803-1347', 'E', 53),
          ('20121021-3193', N'English', '19880117-3333', 'D', 86),
          ('20121021-3193', N'Biology', '19931109-2705', 'E', 96),
          ('20121021-3193', N'Physical Education', '19931109-2705', 'E', 73),
          ('20121021-3193', N'Swedish', '19720630-9697', 'D', 71),
          ('20121208-3065', N'Physical Education', '19931109-2705', 'D', 75),
          ('20121208-3065', N'Mathematics', '19870221-4134', 'E', 42),
          ('20121208-3065', N'English', '19900803-1347', 'D', 57),
          ('20121208-3065', N'History', '19750412-1111', 'E', 61),
          ('20120709-9191', N'Physical Education', '19931109-2705', 'E', 85),
          ('20120709-9191', N'Physics', '19870221-4134', 'B', 97),
          ('20120709-9191', N'Swedish', '19880117-3333', 'E', 40),
          ('20120709-9191', N'Mathematics', '19820930-2222', 'C', 19),
          ('20120422-5104', N'Mathematics', '19870221-4134', 'F', 10),
          ('20120422-5104', N'Physical Education', '19931109-2705', 'D', 81),
          ('20120422-5104', N'Swedish', '19880117-3333', 'F', 90),
          ('20120422-5104', N'Biology', '19931109-2705', 'D', 52),
          ('20110427-1448', N'Swedish', '19720630-9697', 'C', 23),
          ('20110427-1448', N'Physical Education', '19931109-2705', 'B', 72),
          ('20110427-1448', N'Mathematics', '19820930-2222', 'C', 59),
          ('20110427-1448', N'History', '19750412-1111', 'D', 86),
          ('20110709-1835', N'History', '19720630-9697', 'E', 93),
          ('20110709-1835', N'Physical Education', '19931109-2705', 'E', 58),
          ('20110709-1835', N'Physics', '19810512-9111', 'F', 61),
          ('20110709-1835', N'English', '19900803-1347', 'C', 97),
          ('20111023-5023', N'Swedish', '19880117-3333', 'C', 57),
          ('20111023-5023', N'Physics', '19810512-9111', 'F', 37),
          ('20111023-5023', N'English', '19880117-3333', 'C', 43),
          ('20111023-5023', N'Biology', '19931109-2705', 'E', 11),
          ('20111116-6078', N'Swedish', '19880117-3333', 'F', 28),
          ('20111116-6078', N'Physical Education', '19931109-2705', 'D', 53),
          ('20111116-6078', N'Physics', '19870221-4134', 'E', 60),
          ('20111116-6078', N'Biology', '19931109-2705', 'B', 27),
          ('20110309-2720', N'Physical Education', '19931109-2705', 'D', 1),
          ('20110309-2720', N'Biology', '19931109-2705', 'C', 97),
          ('20110309-2720', N'Physics', '19870221-4134', 'D', 69),
          ('20110309-2720', N'Mathematics', '19870221-4134', 'F', 70),
          ('20111219-6488', N'Physical Education', '19931109-2705', 'C', 56),
          ('20111219-6488', N'History', '19720630-9697', 'A', 50),
          ('20111219-6488', N'Swedish', '19720630-9697', 'E', 87),
          ('20111219-6488', N'Biology', '19931109-2705', 'F', 22),
          ('20110712-3836', N'Physical Education', '19931109-2705', 'E', 73),
          ('20110712-3836', N'Biology', '19820930-2222', 'B', 83),
          ('20110712-3836', N'Swedish', '19720630-9697', 'C', 60),
          ('20110712-3836', N'Mathematics', '19820930-2222', 'B', 34),
          ('20110916-2105', N'Biology', '19931109-2705', 'D', 98),
          ('20110916-2105', N'English', '19900803-1347', 'C', 97),
          ('20110916-2105', N'Swedish', '19720630-9697', 'C', 11),
          ('20110916-2105', N'Physical Education', '19931109-2705', 'A', 96),
          ('20110205-9035', N'History', '19750412-1111', 'F', 84),
          ('20110205-9035', N'Mathematics', '19820930-2222', 'F', 4),
          ('20110205-9035', N'English', '19880117-3333', 'C', 3),
          ('20110205-9035', N'Swedish', '19880117-3333', 'C', 17),
          ('20111114-8605', N'Biology', '19931109-2705', 'E', 33),
          ('20111114-8605', N'Physics', '19870221-4134', 'C', 78),
          ('20111114-8605', N'Mathematics', '19870221-4134', 'F', 92),
          ('20111114-8605', N'Swedish', '19880117-3333', 'F', 21),
          ('20120428-2261', N'Physics', '19810512-9111', 'A', 75),
          ('20110822-2345', N'Physical Education', '19931109-2705', 'F', 74)
     ) AS src (student_personal_number, course_name, teacher_personal_number, grade_mark, days_ago)
         JOIN Students s  ON s.student_personal_number = src.student_personal_number
         JOIN Courses c   ON c.course_name = src.course_name
         JOIN Employees t ON t.employee_personal_number = src.teacher_personal_number;
IF @@ROWCOUNT <> 150 THROW 50008, 'Grades: unexpected row count', 1;

COMMIT TRANSACTION;
PRINT 'Seed data inserted successfully.';
END TRY
BEGIN CATCH
IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

SELECT (SELECT COUNT(*) FROM Employees) AS employees,  -- 12
       (SELECT COUNT(*) FROM Students)  AS students,   -- 38
       (SELECT COUNT(*) FROM Groups)    AS groups,     -- 6
       (SELECT COUNT(*) FROM Courses)   AS courses,    -- 8
       (SELECT COUNT(*) FROM Grades)    AS grades;     -- 156


ALTER TABLE Students ALTER COLUMN student_name    NVARCHAR(30) COLLATE Finnish_Swedish_CI_AS NOT NULL;
ALTER TABLE Students ALTER COLUMN student_surname NVARCHAR(50) COLLATE Finnish_Swedish_CI_AS NOT NULL;