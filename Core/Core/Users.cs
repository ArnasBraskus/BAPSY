using Microsoft.Data.Sqlite;

public class Users
{
    Database DB;

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

    public void AddUser(string email, string name)
    {
        DB.ExecuteNonQuery(@"INSERT INTO USERS (email, name) VALUES ($email, $name)", new Dictionary<string, dynamic> { { "$email", email }, { "$name", name } });
    }
}
