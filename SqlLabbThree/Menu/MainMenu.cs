using System.Text;

namespace SqlLabbThree.Menu;

public class MainMenu
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
        bool exit = false;

        Console.WriteLine(_menuTitle);
        
        for (int index = 0; index < MenuOptionCount; index++)
        {
            Console.WriteLine($"{index + 1}. {_menuOptions[index]}");
        }

        int userInput = ReadInteger("Välj ett alternativ: ", 1, MenuOptionCount, default);

        HandleUserInput(userInput);
    }
    
    private static int ReadInteger(string question, int min, int max, string errorMessage = null!)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

        errorMessage ??= $"Ogiltig inmatning. Ange ett tal mellan {min} och {max}.";
        Console.Write(question);

        int result;

        while (!int.TryParse(ReadLineOrThrow(), out result) || result < min || result > max)
        {
            Console.WriteLine(errorMessage);
            Console.Write(question);
        }

        return result;
    }
    
    private static string ReadLineOrThrow()
    {
        return Console.ReadLine() ?? throw new EndOfStreamException("Inmatningen avslutades oväntat.");
    }
    
    private static void HandleUserInput(int userInput)
    {
        switch (userInput)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case MenuOptionCount:
                break;
        }
    }
}