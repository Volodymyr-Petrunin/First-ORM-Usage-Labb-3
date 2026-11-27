using Microsoft.EntityFrameworkCore;
using SqlLabbThree.Data;
using SqlLabbThree.Menu;

namespace SqlLabbThree;

class Program
{
    static void Main(string[] args)
    {
        var dbContextOptions = new DbContextOptionsBuilder<SchoolContext>()
            .UseSqlServer(DatabaseSettings.GetConnectionString())
            .Options;
        
        EnsureConnection(dbContextOptions);

        var consoleInputValidation = new ConsoleInputValidation();
        var mainMenu = new MainMenu(dbContextOptions, consoleInputValidation);
        mainMenu.Show();
    }
    
    private static void EnsureConnection(DbContextOptions<SchoolContext> dbContextOptions)
    {
        using var context = new SchoolContext(dbContextOptions);
        
        try
        {
            context.Database.OpenConnection();
            context.Database.CloseConnection();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Could not connect to database", ex);
        }
    }
}