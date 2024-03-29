using Microsoft.Data.Sqlite;
using System.Net.Mail;

public class Users
{
    private Database DB;

    public Users(Database db)
    {
        DB = db;
    }

    public bool UserExists(string email)
    {
        var parameters = new Dictionary<string, dynamic> {
            { "$email", email }
        };

        var reader = DB.ExecuteSingle(@"SELECT COUNT(1) FROM users WHERE email = $email", parameters);

        if (reader == null)
            return false;

        return reader.GetInt32(0) == 1;
    }

    public User? FindUser(string email)
    {
        var parameters = new Dictionary<string, dynamic> {
            { "$email", email }
        };

        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT id, name FROM users WHERE email = $email", parameters);

        if (reader == null)
            return null;

        int id = reader.GetInt32(0);
        string name = reader.GetString(1);

        return new User(id, email, name);
    }

    public User? FindUser(int id) {
        var parameters = new Dictionary<string, dynamic> {
            { "$id", id }
        };

        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT email, name FROM users WHERE id = $id", parameters);

        if (reader == null)
            return null;

        string email = reader.GetString(0);
        string name = reader.GetString(1);

        return new User(id, email, name);

    }

    private bool IsEmailValid(string address) {
        try {
            MailAddress addr = new MailAddress(address);

            return addr.Address == address;
        }
        catch (FormatException) {
            return false;
        }
    }

    public bool AddUser(string email, string name)
    {
        if (email.Length == 0 || name.Length == 0)
            return false;

        if (!IsEmailValid(email))
            return false;

        var parameters = new Dictionary<string, dynamic> {
            { "$email", email },
            { "$name", name }
        };

        try {
            if (DB.ExecuteNonQuery(@"INSERT INTO USERS (email, name) VALUES ($email, $name)", parameters) != 1)
                return false;
        }
        catch (SqliteException e) {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }

    public bool UpdateName(int id, string name) {
        if (name.Length == 0)
            return false;

        var parameters = new Dictionary<string, dynamic> {
            { "$id", id },
            { "$name", name}
        };

        if (DB.ExecuteNonQuery(@"UPDATE users SET name = $name WHERE id = $id", parameters) != 1)
            return false;

        return true;
    }
}
