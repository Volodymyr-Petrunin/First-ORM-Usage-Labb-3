using System.Diagnostics;
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

    private const string ExitTitle = "Exit";

    private readonly string _menuTitle = new string('=', 10) + "School Database" + new string('=', 10);

    private readonly List<string> _menuOptions =
    [
        ShowAllStudentsTitle,
        ShowAllStudentsInClassTitle,
        AddNewEmployeeTitle,
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
                Console.WriteLine(separatorLine);
                return true; 
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
}