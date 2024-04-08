public class User
{
    private Users Users;
    public int Id { get; }
    public string Email { get; }

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

    public User(Users users, int id, string email, string name)
    {
        Users = users;
        Id = id;
        Email = email;
        _Name = name;
    }
};
