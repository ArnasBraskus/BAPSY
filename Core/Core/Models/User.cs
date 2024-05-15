namespace Core;

public class User
{
    private readonly Users Users;
    public int Id { get; }
    public string Email { get; }
    public string Secret { get; }
    public int SecretVersion { get; }

    private string _Name;
    public string Name
    {
        get
        {
            return _Name;
        }
        set
        {
            Users.UpdateName(Id, value);
            _Name = value;
        }
    }

    public User(Users users, int id, string secret, int secretVer, string email, string name)
    {
        Users = users;
        Id = id;
        Secret = secret;
        SecretVersion = secretVer;
        Email = email;
        _Name = name;
    }
};
