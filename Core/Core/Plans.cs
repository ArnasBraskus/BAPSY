using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;

public class Plans
{
    Database DB;

    public Plans(Database db)
    {
        DB = db;
    }

    public bool AddPlan(User user, string deadLine, int weekdays, string timeOfDay, int pagesPerDay,
            string title, string author, int pageCount, int size)
    {
        if (pagesPerDay < 0 || pageCount < 0 || size < 0) { return false; }
        Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic> {
            { "$userid", user.Id },
            { "$deadline", deadLine },
            { "$weekdays", weekdays },
            { "$timeOfDay", timeOfDay },
            { "$pagesPerDay", pagesPerDay },
            { "$title", title },
            { "$author", author },
            { "$pageCount", pageCount },
            { "$size", size }
        };

        try
        {
            DB.ExecuteNonQuery(@"INSERT INTO PLANS (userid, deadline, weekdays, timeOfDay, pagesPerDay, title, author, pageCount, size) VALUES ($userid, $deadline, $weekdays, $timeOfDay, $pagesPerDay, $title, $author, $pageCount, $size)", dictionary);
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }

    public BookPlan? FindPlan(int id)
    {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT userid, deadline, weekdays, timeOfDay, pagesPerDay, title, author, pageCount, size FROM plans WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

        if (reader == null)
            return null;

        int userid = reader.GetInt32(0);
        string deadline = reader.GetString(1);
        int weekdays = reader.GetInt32(2);
        string timeOfDay = reader.GetString(3);
        int pagesPerDay = reader.GetInt32(4);
        string title = reader.GetString(5);
        string author = reader.GetString(6);
        int pageCount = reader.GetInt32(7);
        int size = reader.GetInt32(8);

        return new BookPlan(id, userid, deadline, weekdays, timeOfDay, pagesPerDay, title, author, pageCount, size);
    }

    public List<int> FindPlanByUser(int userid)
    {
        List<int> ids = new List<int>();
        var readers = DB.Execute(@"SELECT id FROM plans WHERE userid = $userid", new Dictionary<string, dynamic> { { "$userid", userid } });

        foreach (var reader in readers)
        {
            ids.Add(reader.GetInt32(0));
        }
        if (ids.Count == 0) { return null; }
        return ids;

    }

    public bool UpdatePlan(int id, string deadLine, int weekdays, string timeOfDay, string title, string author, int pageCount, int size)
    {
        if (id < 0 || pageCount < 0 || size < 0)
        {
            return false;
        }
        var parameters = new Dictionary<string, dynamic> {
            { "$id", id },
            { "$deadline", deadLine },
            { "$weekdays", weekdays },
            { "$timeOfDay", timeOfDay },
            { "$title", title },
            { "$author", author },
            { "$pageCount", pageCount },
            { "$size", size }
        };

        try
        {
            DB.ExecuteNonQuery(@"UPDATE PLANS SET deadline = $deadline, weekdays = $weekdays, timeOfDay = $timeOfDay, title = $title, author = $author, pageCount = $pageCount, size = $size WHERE id = $id", parameters);
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }


    public bool UpdatePlanDates(int id, string deadLine, int weekdays, string timeOfDay, int pagesPerDay)
    {
        Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic> {
           
            { "$deadline", deadLine },
            { "$weekdays", weekdays },
            { "$timeOfDay", timeOfDay },
            { "$pagesPerDay", pagesPerDay }, 
            { "$id", id }
        };

        try
        {
            DB.ExecuteNonQuery(@"UPDATE plans 
                                 SET deadline = $deadline, weekdays = $weekdays, timeOfDay = $timeOfDay, pagesPerDay = $pagesPerDay 
                                 WHERE id=$id", dictionary);
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
        return true;
    }
    public bool UpdatePlanBook(int id, int pagesPerDay,
            string title, string author, int pageCount, int size)
    {
        Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic> {
           
            { "$pagesPerDay", pagesPerDay },
            { "$title", title },
            { "$author", author },
            { "$pageCount", pageCount },
            { "$size", size },
            { "$id", id }
        };

        try
        {
            DB.ExecuteNonQuery(@"UPDATE plans 
                                SET pagesPerDay = $pagesPerDay, title = $title, author = $author, pageCount = $pageCount, size = $size
                                WHERE id = $id", dictionary);
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
        return true;
    }

    public bool DeletePlan(int id)
    {
        if (id < 0 || FindPlan(id) == null)
            return false;
        try
        {
            DB.ExecuteNonQuery(@"DELETE FROM plans WHERE id=$id ", new Dictionary<string, dynamic>{ { "$id", id } });
        }
        catch (SqliteException e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
        return true;
    }


}
