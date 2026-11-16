using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SqlLabbThree.Data;
using SqlLabbThree.Services;
using SqlLabbThree.Services.Direction;

namespace SqlLabbThree.Menu;

public class MainMenu(DbContextOptions<SchoolContext> dbContextOptions)
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

            userInput = ReadInteger("Choose an option: ", 1, exitOptionIndex);
            
            if (userInput == exitOptionIndex)
            {
                Console.Clear();
                Console.WriteLine("Exiting the program...");
                break;
            }
            
        } while (HandleAndContinue(userInput, exitOptionIndex));
    }
    
    private int ReadInteger(string question, int min, int max)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

        Console.WriteLine();
        Console.Write(question);

        int result;

        while (!int.TryParse(ReadLineOrThrow(), out result) || result < min || result > max)
        {
            Console.WriteLine($"Invalid input. Enter a number between {min} and {max}.");
            Console.Write(question);
        }

        return result;
    }
    
    private static string ReadLineOrThrow()
    {
        return Console.ReadLine() ?? throw new EndOfStreamException("Input was unexpectedly terminated.");
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
                Console.WriteLine(separatorLine);
                return true;
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

        var readSortOptions = ReadSortOptions();

        studentsService.GetAllStudents(readSortOptions.Field, readSortOptions.Direction).ForEach(Console.WriteLine);
        Console.WriteLine(separatorLine);
        return true;
    }

    private (StudentSortField Field, SortDirection Direction) ReadSortOptions()
    {
        Console.WriteLine();
        Console.WriteLine("Sort by:");
        Console.WriteLine("1. First name");
        Console.WriteLine("2. Last name");
        StudentSortField field = ReadInteger("Choose: ", 1, 2) == 1
            ? StudentSortField.FirstName
            : StudentSortField.LastName;

        Console.WriteLine();
        Console.WriteLine("Order:");
        Console.WriteLine("1. Ascending");
        Console.WriteLine("2. Descending");
        SortDirection direction = ReadInteger("Choose: ", 1, 2) == 1
            ? SortDirection.Ascending
            : SortDirection.Descending;

        return (field, direction);
    }
}