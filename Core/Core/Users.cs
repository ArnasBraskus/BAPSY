using Microsoft.Data.Sqlite;

public class Users
{
    public Database DB;

    public Users(Database db)
    {
        DB = db;
    }

    public bool UserExists(string email)
    {
        var reader = DB.ExecuteSingle(@"SELECT COUNT(1) FROM users WHERE email = $email", new Dictionary<string, dynamic> { { "$email", email } });

        if (reader == null)
            return false;

        return reader.GetInt32(0) == 1;
    }

    public User? FindUser(string email)
    {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT id, name FROM users WHERE email = $email", new Dictionary<string, dynamic> { { "$email", email } });

        if (reader == null)
            return null;

        int id = reader.GetInt32(0);
        string name = reader.GetString(1);

        return new User(id, email, name);
    }

    public User? FindUser(int id) {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT email, name FROM users WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

        if (reader == null)
            return null;

        string email = reader.GetString(0);
        string name = reader.GetString(1);

        return new User(id, email, name);

    }

    public bool AddUser(string email, string name)
    {
        if (email.Length == 0 || name.Length == 0)
            return false;

        try {
            DB.ExecuteNonQuery(@"INSERT INTO USERS (email, name) VALUES ($email, $name)", new Dictionary<string, dynamic> { { "$email", email }, { "$name", name } });
        }
        catch (SqliteException e) {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }
}
