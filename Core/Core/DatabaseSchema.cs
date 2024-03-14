public static class DatabaseSchema
{
    public static readonly string Schema =
    @"
    CREATE TABLE users(
        id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
        email TEXT UNIQUE NOT NULL,
        name TEXT NOT NULL);
    ";
}
