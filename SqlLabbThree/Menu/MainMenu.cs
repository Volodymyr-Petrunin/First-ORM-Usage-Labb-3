using System.Diagnostics;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SqlLabbThree.Data;
using SqlLabbThree.Domain;
using SqlLabbThree.Services;

namespace SqlLabbThree.Menu;

public class MainMenu(DbContextOptions<SchoolContext> dbContextOptions, ConsoleInputValidation inputValidation)
{

    private const string ShowAllStudentsTitle = "Show all students";

    private const string ShowAllStudentsInClassTitle = "Show all students in class";

    private const string AddNewEmployeeTitle = "Add new employee";

    private const string CountTeachersInDepartmentTitle = "Count teachers in department";

    private const string ListOfAllActiveCoursesTitle = "List of all active courses";

    private const string UpdateStudentInfoTitle = "Update student info";

    private const string ExitTitle = "Exit";

    private readonly string _menuTitle = new string('=', 10) + "School Database" + new string('=', 10);

    private readonly List<string> _menuOptions =
    [
        ShowAllStudentsTitle,
        ShowAllStudentsInClassTitle,
        AddNewEmployeeTitle,
        CountTeachersInDepartmentTitle,
        ListOfAllActiveCoursesTitle,
        UpdateStudentInfoTitle,
    ];

    public void Show()
    {
        int exitOptionIndex = _menuOptions.Count + 1;
        int userInput;
        
        do
        {
            Console.WriteLine(_menuTitle);
        
            for (int index = 0; index < _menuOptions.Count; index++)
            {
                Console.WriteLine($"{index + 1}. {_menuOptions[index]}");
            }
            
            Console.WriteLine($"{exitOptionIndex}. {ExitTitle}");

            userInput = inputValidation.ReadInteger("Choose an option: ", 1, exitOptionIndex);
            
            if (userInput == exitOptionIndex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Exiting the program...");
                break;
            }
            
        } while (HandleAndContinue(userInput, exitOptionIndex));
    }
    
    private bool HandleAndContinue(int userInput, int exitOptionIndex)
    {
        string separatorLine = new string('-', Console.WindowWidth);
        
        switch (userInput)
        {
            case 1:
            {
                return HandleFindAllStudents(separatorLine);
            }
            case 2:
            {
                return HandleFindAllStudentsInGroup(separatorLine);
            }
            case 3:
            {
                return HandleCreateEmployee(separatorLine);
            }
            case 4:
            {
                return HandleCountTeachersInDepartment(separatorLine);
            }
            case 5:
            {
                return HandleListOfAllActiveCourses(separatorLine);
            }
            case 6:
            {
                return HandleUpdateStudent(separatorLine);
            }
            default:
            {
                throw new UnreachableException($"Invalid input. Enter a number between 1 and {exitOptionIndex}."); 
            }
        }
    }

    private bool HandleFindAllStudents(string separatorLine)
    {
        using var context = new SchoolContext(dbContextOptions);
        var studentsService = new StudentsService(context);

        Console.WriteLine("How to group students?");

        var readSortOptions = inputValidation.ReadSortOptions();

        studentsService.GetAllStudents(readSortOptions.Field, readSortOptions.Direction).ForEach(Console.WriteLine);
        Console.WriteLine(separatorLine);
        return true;
    }

    private bool HandleFindAllStudentsInGroup(string separatorLine)
    {
        using var context = new SchoolContext(dbContextOptions);
        var studentsService = new StudentsService(context);
        var groupService = new GroupService(context);

        int groupId = PickGroup(groupService);
        var readSortOptions = inputValidation.ReadSortOptions();

        studentsService.GetStudentsInGroup(groupId, readSortOptions.Field, readSortOptions.Direction).ForEach(Console.WriteLine);
        
        Console.WriteLine(separatorLine);
        return true;
    }

    private int PickGroup(GroupService groupService)
    {
        List<Group> allGroups = groupService.GetAllGroups();
        int index = 1;
        
        foreach (Group group in allGroups)
        {
            Console.WriteLine($"{index++} {group.GroupId}: {group.GroupName}");
        }

        int groupNumber = inputValidation.ReadInteger("Enter group number: ", 1, allGroups.Count);

        return allGroups[groupNumber - 1].GroupId;
    }
    
    private bool HandleCreateEmployee(string separatorLine)
    {
        using var context = new SchoolContext(dbContextOptions);
        var employeeService = new EmployeeService(context);

        Employee employee = inputValidation.ReadEmployee();

        List<Department> departments = context.Set<Department>()
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .ToList();

        Console.WriteLine();
        for (int index = 0; index < departments.Count; index++)
        {
            Console.WriteLine($"{index + 1}. {departments[index].Name}");
        }

        int choice = inputValidation.ReadInteger("Choose department: ", 1, departments.Count);
        employee.DepartmentId = departments[choice - 1].Id;
        employee.Salary = ReadSalary("Enter monthly salary (SEK): ");
        employee.StartDay = DateOnly.FromDateTime(DateTime.Today);
        employee.EndDay = null;

        try
        {
            Employee newEmployee = employeeService.CreateNewEmployee(employee);
            Console.WriteLine("New Employee Created");
            Console.WriteLine(newEmployee);
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Could not save employee: {ex.InnerException?.Message ?? ex.Message}");
        }

        Console.WriteLine(separatorLine);
        return true;
    }

    private static decimal ReadSalary(string question)
    {
        const decimal maxSalary = 99_999_999.99m;

        while (true)
        {
            Console.Write(question);
            string? input = Console.ReadLine()?.Trim().Replace(',', '.');

            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal salary)
                && salary > 0 && salary <= maxSalary)
            {
                return salary;
            }

            Console.WriteLine($"Invalid salary. Enter a number between 0.01 and {maxSalary}.");
        }
    }

    private bool HandleCountTeachersInDepartment(string separatorLine)
    {
        using var context = new SchoolContext(dbContextOptions);
        var departmentService = new DepartmentService(context);
        var teachersInDepartment = departmentService.CountTeachersInDepartment();
        
        foreach (var (departmentName, teachersCount) in teachersInDepartment)
        {
            Console.WriteLine($"{departmentName}: {teachersCount}");
        }
        
        Console.WriteLine(separatorLine);
        return true;
    }

    private bool HandleListOfAllActiveCourses(string separatorLine)
    {
        using var context = new SchoolContext(dbContextOptions);
        var courseService = new CourseService(context);
        List<Course> activeCourses = courseService.GetAllActiveCourses();

        foreach (Course course in activeCourses)
        {
            Console.WriteLine($"{course.CourseName} - {course.StartDay} - {course.EndDay?.ToString() ?? "No end day was set"}");
        }

        Console.WriteLine(separatorLine);
        return true;
    }

    private bool HandleUpdateStudent(string separatorLine)
    {
        using var context = new SchoolContext(dbContextOptions);
        var studentsService = new StudentsService(context);
        var groupService = new GroupService(context);

        Student? foundStudent = studentsService
            .FindStudentById(inputValidation.ReadInteger("Enter the id of the student to update: ", 1, int.MaxValue));
        
        if (foundStudent == null)
        {
            Console.WriteLine("Student not found");
            return true;
        }
        
        UpdateStudentNameAndSurname(foundStudent);
        UpdateStudentPersonalNumber(foundStudent, studentsService);
        UpdateStudentGroup(foundStudent, groupService);

        Student updatedStudent = studentsService.UpdateStudent(foundStudent);
        Console.WriteLine("Student updated: " + updatedStudent.ToString());
        Console.WriteLine(separatorLine);
        return true;
    }
    
    private void UpdateStudentNameAndSurname(Student foundStudent)
    {
        Console.WriteLine($"Updating student: {foundStudent.StudentName} {foundStudent.StudentSurname}");
        Console.WriteLine("(Press Enter to keep current value)");

        Console.Write($"New first name [{foundStudent.StudentName}]: ");
        string? newFirstName = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(newFirstName))
        {
            foundStudent.StudentName = newFirstName.Trim();
        }

        Console.Write($"New last name [{foundStudent.StudentSurname}]: ");
        string? newLastName = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(newLastName))
        {
            foundStudent.StudentSurname = newLastName.Trim();
        }
    }
    
    private void UpdateStudentPersonalNumber(Student foundStudent, StudentsService studentsService)
    {
        Console.WriteLine($"Updating student personal number: {foundStudent.StudentPersonalNumber}");

        string newPersonalNumber = inputValidation
            .ReadPersonalNumber($"New personal number [{foundStudent.StudentPersonalNumber}]: ");

        if (studentsService.IsPersonalNumberValid(newPersonalNumber))
        {
            foundStudent.StudentPersonalNumber = newPersonalNumber.Trim();
        } else
        {
            Console.WriteLine("Personal number already in use or not valid. And was not updated.");
        }
    }
    
    private void UpdateStudentGroup(Student foundStudent, GroupService groupService)
    {
        Console.WriteLine($"Updating student group: {foundStudent.StudentGroup.GroupName}");

        List<Group> groups = groupService.GetAllGroups();
        
        for (int index = 0; index < groups.Count; index++)
        {
            Console.WriteLine($"{index + 1}. {groups[index].GroupName}");
        }

        int choice = inputValidation.ReadInteger("Choose group: ", 1, groups.Count);
        foundStudent.StudentGroup = groups[choice - 1];
    }
}