namespace Core;

using Microsoft.Data.Sqlite;
using System.Net.Mail;

public class Users
{
    private readonly Database DB;

    public Users(Database db)
    {
        DB = db;
    }

    public bool UserExists(string email)
    {
        var parameters = new Dictionary<string, dynamic> {
            { "$email", email }
        };

        var reader = DB.ExecuteSingle(@"SELECT 1 FROM users WHERE email = $email", parameters);

        if (reader == null)
            return false;

        return true;
    }

    public User FindUser(string email)
    {
        var parameters = new Dictionary<string, dynamic> {
            { "$email", email }
        };

        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT id, secret, secret_ver, name FROM users WHERE email = $email", parameters);

        if (reader == null)
            throw new KeyNotFoundException("User not found");

        int id = reader.GetInt32(0);
        string secret = reader.GetString(1);
        int secretVer = reader.GetInt32(2);
        string name = reader.GetString(3);

        return new User(this, id, secret, secretVer, email, name);
    }

    public User FindUser(int id)
    {
        var parameters = new Dictionary<string, dynamic> {
            { "$id", id }
        };

        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT secret, secret_ver, email, name FROM users WHERE id = $id", parameters);

        if (reader == null)
            throw new KeyNotFoundException("User not found");

        string secret = reader.GetString(0);
        int secretVer = reader.GetInt32(1);
        string email = reader.GetString(2);
        string name = reader.GetString(3);

        return new User(this, id, secret, secretVer, email, name);

    }

    private static bool IsEmailValid(string address)
    {
        try
        {
            MailAddress addr = new MailAddress(address);

            return addr.Address == address;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public void AddUser(string email, string name)
    {
        if (email.Length == 0)
            throw new ArgumentException("Email is empty");

        if (name.Length == 0)
            throw new ArgumentException("Name is empty");

        if (!IsEmailValid(email))
            throw new FormatException("Email is not valid");

        var secret = AuthUtils.GenerateSecret();
        var secretVer = 0;

        var parameters = new Dictionary<string, dynamic> {
            { "$email", email },
            { "$name", name },
            { "$secret", secret },
            { "$secret_ver", secretVer }
        };

        DB.ExecuteNonQuery(@"INSERT INTO USERS (email, name, secret, secret_ver) VALUES ($email, $name, $secret, $secret_ver)", parameters);
    }

    public void UpdateName(int id, string name)
    {
        if (name.Length == 0)
            throw new ArgumentException("Name is empty");

        var parameters = new Dictionary<string, dynamic> {
            { "$id", id },
            { "$name", name}
        };

        if (DB.ExecuteNonQuery(@"UPDATE users SET name = $name WHERE id = $id", parameters) != 1)
            throw new InvalidOperationException("User not found");
    }
}
