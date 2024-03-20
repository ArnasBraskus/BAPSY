using Microsoft.Data.Sqlite;
public class Books
{
    Database DB;

    public Books(Database db)
    {
        DB = db;
    }

    public bool BookExists(int id)
    {
        var reader = DB.ExecuteSingle(@"SELECT COUNT(1) FROM books WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

        if (reader == null)
            return false;

        return reader.GetInt32(0) == 1;
    }
    public bool AddBook(int planid, string title, string author, int pageCount, int size)
    {

        try
        {
            DB.ExecuteNonQuery(@"INSERT INTO books (planid, title, author, pageCount, size) VALUES ($planid, $title, $author, $pageCount, $size)", new Dictionary<string, dynamic> { { "$planid", planid }, { "$title", title },
                
                { "$author", author }, { "$pageCount", pageCount}, { "$size", size } });
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }

    public Book? FindBook(int id)
    {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT planid, title, author, pageCount, size FROM books WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

        if (reader == null)
            return null;

        int planid = reader.GetInt32(0);
        string title = reader.GetString(1);
        string author = reader.GetString(2);
        int pageCount = reader.GetInt32(3);
        int size = reader.GetInt32(4);


        return new Book(id, planid, title, author, pageCount, size);
    }

    public Book? FindBookByPlan(int planId)
    {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT id, title, author, pageCount, size FROM books WHERE planid = $planid", new Dictionary<string, dynamic> { { "$planid", planId } });

        if (reader == null)
            return null;

        int id = reader.GetInt32(0);
        string title = reader.GetString(1);
        string author = reader.GetString(2);
        int pageCount = reader.GetInt32(3);
        int size = reader.GetInt32(4);


        return new Book(id, planId, title, author, pageCount, size);

    }
}

