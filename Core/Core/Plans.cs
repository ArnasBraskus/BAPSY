using Core;
using Microsoft.Data.Sqlite;

public class Plans
{
    Database DB;

    public Plans(Database db)
    {
        DB = db;
    }

    public bool PlanExists(int id)
    {
        var reader = DB.ExecuteSingle(@"SELECT COUNT(1) FROM plans WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

        if (reader == null)
            return false;

        return reader.GetInt32(0) == 1;
    }
    public bool AddPlan(int userid, int hoursPerDay, DateTime deadline, int weekdays, int pagesPerDay)
    {
        if (pagesPerDay == 0 || userid == 0)
            return false;

        try
        {
            DB.ExecuteNonQuery(@"INSERT INTO PLANS (userid, deadline, weekdays, pagesPerDay) VALUES ($userid, $deadline, $weekdays, $pagesPerDay)", new Dictionary<string, dynamic> { { "$userid", userid }, { "$deadline", deadline }, { "$weekdays", weekdays }, { "$pagesPerDay", pagesPerDay }});
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
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT userid, deadline, weekdays, pagesPerDay FROM plans WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

        if (reader == null)
            return null;

        int userid = reader.GetInt32(0);
        DateTime deadline = reader.GetDateTime(1);
        int weekdays = reader.GetInt32(2);
        int pagesPerDay = reader.GetInt32(3);

        return new BookPlan(id, userid, deadline, weekdays, pagesPerDay);
    }

    public BookPlan? FindPlanByUser(int userid)
    {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT id, deadline, weekdays, pagesPerDay FROM plans WHERE userid = $userid", new Dictionary<string, dynamic> { { "$userid", userid } });

        if (reader == null)
            return null;

        int id = reader.GetInt32(0);
        DateTime deadline = reader.GetDateTime(1);
        int weekdays = reader.GetInt32(2);
        int pagesPerDay = reader.GetInt32(3);
        return new BookPlan(id, userid, deadline, weekdays, pagesPerDay);

    }

}
