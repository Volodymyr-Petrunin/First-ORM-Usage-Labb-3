using SqlLabbThree.Domain;
using SqlLabbThree.Services.Direction;

namespace SqlLabbThree.Menu;

public class ConsoleInputValidation
{

    private const string PersonalNumberFormat = "00000000-0000";
    
    public int ReadInteger(string question, int min, int max)
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
    
    public (StudentSortField Field, SortDirection Direction) ReadSortOptions()
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

    public Employee ReadEmployee()
    {
        Console.WriteLine("Enter Employee Name");
        string employeeName = ReadLineOrThrow();

        Console.WriteLine("Enter Employee Surname");
        string employeeSurname = ReadLineOrThrow();
        
        string employeePersonalNumber = ReadPersonalNumber("Enter Personal Number");
        
        return new Employee(employeeName, employeeSurname, employeePersonalNumber);
    }

    private string ReadPersonalNumber(string question)
    {
        Console.WriteLine(question);

        string personalNumber = ReadLineOrThrow();
        
        while (!IsPersonalNumberValid(personalNumber))
        {
            Console.WriteLine("Invalid personal number. Format: YYYYMMDD-XXXX");
            personalNumber = ReadLineOrThrow();
        }
        
        return personalNumber;
    }

    private bool IsPersonalNumberValid(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length != PersonalNumberFormat.Length)
        {
            return false;
        }

        for (int index = 0; index < input.Length; index++)
        {
            if (PersonalNumberFormat[index] == '0' && !char.IsDigit(input[index]))
            {
                return false;
            }

            if (PersonalNumberFormat[index] == '-' && input[index] != '-')
            {
                return false;
            }
        }
        
        return true;
    }
    
    private static string ReadLineOrThrow()
    {
        return Console.ReadLine() ?? throw new EndOfStreamException("Input was unexpectedly terminated.");
    }
}