using static System.Net.Mime.MediaTypeNames;

public static class DatabaseSchema
{
    public static readonly string Schema =
    @"
    CREATE TABLE users(
        id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
        email TEXT UNIQUE NOT NULL,
        name TEXT NOT NULL);
    ";
    public static readonly string PlansTable =
    @"
    CREATE TABLE plans(
        id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
        userId INTEGER NOT NULL,
        deadline DATE NOT NULL,
        weekdays INTEGER NOT NULL,
        pagesPerDay INTEGER NOT NULL,
        FOREIGN KEY (userid) REFERENCES users(id),);
        
    ";
    public static readonly string BooksTable =
   @"CREATE TABLE books (
	    id	PRIMARY KEY INTEGER NOT NULL,
	    name TEXT NOT NULL,
	    author TEXT NOT NULL,
	    pageCount INTEGER NOT NULL,
	    size INTEGER,
        planId INTEGER,
        FOREIGN KEY (planid) REFERENCES plans(id),);
	";
}
