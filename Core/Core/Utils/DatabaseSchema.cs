public static class DatabaseSchema
{
    public static readonly string Schema =
    @"
    CREATE TABLE users(
        id INTEGER PRIMARY KEY NOT NULL,
        secret TEXT NOT NULL,
        secret_ver INTEGER NOT NULL,
        email TEXT UNIQUE NOT NULL,
        name TEXT NOT NULL);

    CREATE TABLE plans(
        id INTEGER PRIMARY KEY NOT NULL,
        userId INTEGER NOT NULL,
        deadline TEXT NOT NULL,
        weekdays INTEGER NOT NULL,
        timeOfDay TEXT NOT NULL,
        pagesPerDay INTEGER NOT NULL,
        title TEXT NOT NULL,
	    author TEXT NOT NULL,
	    pageCount INTEGER NOT NULL,
        pagesRead INTEGER DEFAULT 0,
        FOREIGN KEY (userid) REFERENCES users(id));

    CREATE TABLE readingsessions(
        id INTEGER PRIMARY KEY NOT NULL,
        planId INTEGER NOT NULL,
        date TEXT NOT NULL,
        goal INTEGER NOT NULL,
        actual INTEGER DEFAULT 0,
        completed INTEGER DEFAULT 0,
        FOREIGN KEY (planId) REFERENCES plans(id));
    ";
}
