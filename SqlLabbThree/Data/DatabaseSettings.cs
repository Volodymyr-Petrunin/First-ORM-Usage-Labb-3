namespace SqlLabbThree.Data;

public static class DatabaseSettings
{
    public const string ConnectionStringVariable = "SCHOOL_DB";

    public static string GetConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Environment variable {ConnectionStringVariable} is not set");
        }

        return connectionString;
    }
}