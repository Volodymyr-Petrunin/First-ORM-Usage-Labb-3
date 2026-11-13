using Microsoft.EntityFrameworkCore;
using SqlLabbThree.Data;
using SqlLabbThree.Services;

namespace SqlLabbThree.Menu;

public class MainMenu(DbContextOptions<SchoolContext> dbContextOptions)
{

    private const string ShowAllStudentsTitle = "Show all students";

    private const string ShowAllStudentsInClassTitle = "Show all students in class";

    private const string AddNewEmployeeTitle = "Add new employee";

    private const string ExitTitle = "Exit";

    private const byte MenuOptionCount = 4;

    private readonly string _menuTitle = new string('=', 10) + "School Database" + new string('=', 10);

    private readonly List<string> _menuOptions =
    [
        ShowAllStudentsTitle,
        ShowAllStudentsInClassTitle,
        AddNewEmployeeTitle,
        ExitTitle,
    ];

    public void Show()
    {
        int userInput;
        do
        {
            Console.WriteLine(_menuTitle);
        
            for (int index = 0; index < MenuOptionCount; index++)
            {
                Console.WriteLine($"{index + 1}. {_menuOptions[index]}");
            }

            userInput = ReadInteger("Välj ett alternativ: ", 1, MenuOptionCount);
        } while (HandleUserInput(userInput));
    }
    
    private int ReadInteger(string question, int min, int max)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

        Console.WriteLine();
        Console.Write(question);

        int result;

        while (!int.TryParse(ReadLineOrThrow(), out result) || result < min || result > max)
        {
            Console.WriteLine($"Ogiltig inmatning. Ange ett tal mellan {min} och {max}.");
            Console.Write(question);
        }

        return result;
    }
    
    private static string ReadLineOrThrow()
    {
        return Console.ReadLine() ?? throw new EndOfStreamException("Inmatningen avslutades oväntat.");
    }
    
    private bool HandleUserInput(int userInput)
    {
        string separatorLine = new string('-', Console.WindowWidth);
        
        switch (userInput)
        {
            case 1:
            {
                using var context = new SchoolContext(dbContextOptions);
                var studentsService = new StudentsService(context);
                studentsService.GetAllStudents().ForEach(Console.WriteLine);
                Console.WriteLine(separatorLine);
                return true;
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
            case MenuOptionCount:
            {
                return false;
            }
            default:
            {
                Console.WriteLine(separatorLine);
                return true;
            }
        }
    }
}