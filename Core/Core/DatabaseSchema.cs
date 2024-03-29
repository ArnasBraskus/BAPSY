using static System.Net.Mime.MediaTypeNames;

public static class DatabaseSchema
{
    public static readonly string Schema =
    @"
    CREATE TABLE users(
        id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
        email TEXT UNIQUE NOT NULL,
        name TEXT NOT NULL);

    CREATE TABLE plans(
        id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
        userId INTEGER NOT NULL,
        deadline TEXT NOT NULL,
        weekdays INTEGER NOT NULL,
        timeOfDay TEXT NOT NULL,
        pagesPerDay INTEGER NOT NULL,	    
        title TEXT NOT NULL,
	    author TEXT NOT NULL,
	    pageCount INTEGER NOT NULL,
	    size INTEGER NOT NULL,
        FOREIGN KEY (userid) REFERENCES users(id));   
    ";
}