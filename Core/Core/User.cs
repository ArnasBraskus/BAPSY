public class User {
    public int Id { get; }
    public string Email { get; }
    public string Name { get; }

    public User(int id, string email, string name) {
        Id = id;
        Email = email;
        Name = name;
    }
};
