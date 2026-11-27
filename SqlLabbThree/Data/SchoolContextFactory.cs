using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SqlLabbThree.Data;

public class SchoolContextFactory : IDesignTimeDbContextFactory<SchoolContext>
{
    public SchoolContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SchoolContext>()
            .UseSqlServer(DatabaseSettings.GetConnectionString())
            .Options;

        return new SchoolContext(options);
    }
}