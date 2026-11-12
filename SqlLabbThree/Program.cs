using Microsoft.EntityFrameworkCore;
using SqlLabbThree.Data;

namespace SqlLabbThree;

class Program
{
    private const string SchoolDbVariable = "SCHOOL_DB";
    
    private static readonly string? ConnectionString = Environment.GetEnvironmentVariable(SchoolDbVariable);
    
    private static readonly DbContextOptions<SchoolContext> DbContextOptions = new DbContextOptionsBuilder<SchoolContext>()
                                                                   .UseSqlServer(ConnectionString)
                                                                   .Options;
    static void Main(string[] args)
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            throw new InvalidOperationException($"Environment variable {SchoolDbVariable} is not set");
        }

        EnsureConnection();
    }
    
    private static void EnsureConnection()
    {
        using var context = new SchoolContext(DbContextOptions);
        
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