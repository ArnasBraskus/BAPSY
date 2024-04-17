using Microsoft.Data.Sqlite;

public class Database
{
    private SqliteConnection Connection;

    public Database(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
    }

    public bool Empty() {
        return GetUserVersion() == 0;
    }

    public void Create(string schema) {
        ExecuteNonQuery(schema);
        ExecuteNonQuery("PRAGMA user_version = 1");
    }

    public int LastInsertedRowId() {
        return ExecuteScalar("SELECT last_insert_rowid()");
    }

    public void CreateIfEmpty(string schema) {
        if (Empty()) {
            Create(schema);
        }
    }

    public void Open()
    {
        Connection.Open();
    }

    public int ExecuteScalar(string statement) {
        var command = CreateCommand(statement, null);
        object? result = command.ExecuteScalar()!;

        return (int)((long)result);
    }

    private int GetUserVersion() {
        return ExecuteScalar("PRAGMA user_version");
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
        using (var command = CreateCommand(statement, parameters))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return reader;
                }
            }
        }
    }

    public SqliteDataReader? ExecuteSingle(string statement, Dictionary<string, dynamic>? parameters)
    {
        var command = CreateCommand(statement, parameters);
        var reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return reader;
    }
}
