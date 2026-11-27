-- Lab 4: backfill for migration AddDepartmentsAndEmploymentDates.
-- Runs AFTER that migration (new columns exist and are NULL) and BEFORE
-- migration MakeEmploymentFieldsRequired (which sets NOT NULL + CHECK).
-- Same style as seed-data.sql: natural keys only, one transaction,
-- row-count guards, and a final guard that no required value is left NULL.
--
-- Deliberate edge cases:
--   * Department 'Student Health' has no employees (empty group in aggregates).
--   * Anna Lindqvist is Teacher + Principal -> placed in Administration,
--     so Administration contains a teacher.
--   * Peter Sjoberg is Teacher + Administrator -> placed in Languages,
--     so one salary must be counted in exactly one department.
--   * Mikael Strand has left (end date in the past) -> must NOT count as active staff.
--   * Ingrid Akesson leaves in the future (end date after today) -> still active.
--   * Karin Holm was hired on 2019-12-31 -> DATEDIFF(YEAR, ...) overcounts by one.
--   * Course 'Physical Education' has no end date (open-ended).
--   * Course 'Music' starts in the future (not active yet, has no grades).
--   * Course 'German' ended in the past (not active anymore, has no grades).

SET XACT_ABORT ON;

BEGIN TRY
BEGIN TRANSACTION;

    -- Departments
INSERT INTO Departments (department_name) VALUES
    (N'Science'),
    (N'Languages'),
    (N'Practical & Aesthetic'),
    (N'Administration'),
    (N'Facilities'),
    (N'Student Health');
IF @@ROWCOUNT <> 6 THROW 50101, 'Departments: unexpected row count', 1;

    -- Employees: department, monthly salary (SEK), start and end of employment
UPDATE e
SET e.department_id      = d.department_id,
    e.employee_salary    = src.salary,
    e.employee_start_day = src.start_day,
    e.employee_end_day   = src.end_day
FROM Employees e
    JOIN (VALUES
        ('19750412-1111', N'Administration',        62000.00, '2008-08-11', NULL),
        ('19820930-2222', N'Science',               43500.00, '2012-08-13', NULL),
        ('19880117-3333', N'Languages',             41000.00, '2015-08-10', NULL),
        ('19790605-4444', N'Administration',        39500.00, '2010-03-01', NULL),
        ('19920223-5555', N'Facilities',            31000.00, '2019-12-31', NULL),
        ('19810512-9111', N'Science',               42000.00, '2016-08-15', NULL),
        ('19900803-1347', N'Languages',             37500.00, '2021-08-16', NULL),
        ('19870221-4134', N'Science',               40500.00, '2018-01-08', NULL),
        ('19931109-2705', N'Practical & Aesthetic', 36000.00, '2022-08-15', NULL),
        ('19720630-9697', N'Languages',             47000.00, '2001-08-13', NULL),
        ('19680914-2182', N'Administration',        48500.00, '1995-08-14', '2027-06-30'),
        ('19770127-6417', N'Facilities',            30500.00, '2014-04-01', '2026-09-30')
    ) AS src (personal_number, department_name, salary, start_day, end_day)
        ON e.employee_personal_number = src.personal_number
    JOIN Departments d ON d.department_name = src.department_name;
IF @@ROWCOUNT <> 12 THROW 50102, 'Employees: unexpected row count', 1;

    -- Courses: running school year 2026/27 unless stated otherwise
UPDATE c
SET c.course_start_day = src.start_day,
    c.course_end_day   = src.end_day
FROM Courses c
    JOIN (VALUES
        (N'Mathematics',        '2026-08-10', '2027-06-11'),
        (N'Swedish',            '2026-08-10', '2027-06-11'),
        (N'English',            '2026-08-10', '2027-06-11'),
        (N'Biology',            '2026-08-10', '2027-06-11'),
        (N'History',            '2026-08-10', '2027-06-11'),
        (N'Physics',            '2026-08-10', '2027-06-11'),
        (N'Physical Education', '2026-08-10', NULL),
        (N'Music',              '2027-01-11', '2027-06-11')
    ) AS src (course_name, start_day, end_day)
        ON c.course_name = src.course_name;
IF @@ROWCOUNT <> 8 THROW 50103, 'Courses: unexpected row count', 1;

    -- A course that has already ended
INSERT INTO Courses (course_name, course_description, course_start_day, course_end_day) VALUES
    (N'German', N'Grammar, conversation and culture', '2025-08-11', '2026-06-12');
IF @@ROWCOUNT <> 1 THROW 50104, 'German course: unexpected row count', 1;

    -- Final guards: the next migration (NOT NULL + CHECK) must not be able to fail
IF EXISTS (SELECT 1 FROM Employees
           WHERE department_id IS NULL
              OR employee_salary IS NULL
              OR employee_start_day IS NULL)
    THROW 50105, 'Employees: required value still NULL', 1;

IF EXISTS (SELECT 1 FROM Courses WHERE course_start_day IS NULL)
    THROW 50106, 'Courses: start day still NULL', 1;

IF EXISTS (SELECT 1 FROM Employees WHERE employee_salary <= 0
                                      OR employee_end_day < employee_start_day)
    THROW 50107, 'Employees: invalid salary or end before start', 1;

IF EXISTS (SELECT 1 FROM Courses WHERE course_end_day < course_start_day)
    THROW 50108, 'Courses: end before start', 1;

COMMIT TRANSACTION;
PRINT 'Backfill completed successfully.';
END TRY
BEGIN CATCH
IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

SELECT (SELECT COUNT(*) FROM Departments)                                AS departments,           -- 6
       (SELECT COUNT(*) FROM Employees WHERE department_id IS NOT NULL)  AS employees_with_dept,   -- 12
       (SELECT COUNT(*) FROM Courses WHERE course_start_day IS NOT NULL) AS courses_with_start;    -- 9
