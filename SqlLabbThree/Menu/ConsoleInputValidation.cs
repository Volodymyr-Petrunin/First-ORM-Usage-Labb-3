using SqlLabbThree.Services.Direction;

namespace SqlLabbThree.Menu;

public class ConsoleInputValidation
{
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
    
    private static string ReadLineOrThrow()
    {
        return Console.ReadLine() ?? throw new EndOfStreamException("Input was unexpectedly terminated.");
    }
}