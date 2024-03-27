using Microsoft.Data.Sqlite;

public class Database
{
    private SqliteConnection Connection;

    public Database(string path)
    {
        Connection = new SqliteConnection($"Data Source={path}");
    }

    public Database()
    {
        Connection = new SqliteConnection("Data Source=:memory:");
    }

    public void Create(string schema) {
        ExecuteNonQuery(schema);
    }

    public bool Open()
    {
        try
        {
            Connection.Open();
        }
        catch (SqliteException)
        {
            return false;
        }

        return true;
    }

    private SqliteCommand CreateCommand(string statement, Dictionary<string, dynamic>? parameters)
    {
        var command = Connection.CreateCommand();

        command.CommandText = statement;

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        return command;
    }

    public int ExecuteNonQuery(string statement, Dictionary<string, dynamic>? parameters)
    {
        return CreateCommand(statement, parameters).ExecuteNonQuery();
    }

    public int ExecuteNonQuery(string statement)
    {
        return ExecuteNonQuery(statement, null);
    }

    public IEnumerable<SqliteDataReader> Execute(string statement, Dictionary<string, dynamic>? parameters)
    {
        var command = CreateCommand(statement, parameters);

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            yield return reader;
        }
    }

    public IEnumerable<SqliteDataReader> Execute(string statement)
    {
        return Execute(statement, null);
    }

    public SqliteDataReader? ExecuteSingle(string statement, Dictionary<string, dynamic>? parameters)
    {
        IEnumerable<SqliteDataReader> reader = Execute(statement, parameters);

        if (reader.Count() == 0)
            return null;

        return reader.First();
    }

    public SqliteDataReader? ExecuteSingle(string statement)
    {
        return ExecuteSingle(statement);
    }
};
