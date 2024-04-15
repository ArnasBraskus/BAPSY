﻿using Microsoft.Data.Sqlite;

public class Plans
{
    private Database DB;
    private ReadingSessions ReadingSessions;

    public Plans(Database db)
    {
        DB = db;
        ReadingSessions = new ReadingSessions(db);
    }

    public int AddPlan(User user, string deadLine, int weekdays, string timeOfDay, int pagesPerDay,
            string title, string author, int pageCount)
    {
        if (pageCount < 0)
            throw new ArgumentException("Page count must be greater than zero");
        if (title.Length == 0)
            throw new ArgumentException("where title");
        if (author.Length == 0)
            throw new ArgumentException("where author");

        Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic> {
            { "$userid", user.Id },
            { "$deadline", deadLine },
            { "$weekdays", weekdays },
            { "$timeOfDay", timeOfDay },
            { "$pagesPerDay", pagesPerDay },
            { "$title", title },
            { "$author", author },
            { "$pageCount", pageCount }
        };

        DB.ExecuteNonQuery(@"INSERT INTO PLANS (userid, deadline, weekdays, timeOfDay, pagesPerDay, title, author, pageCount) VALUES ($userid, $deadline, $weekdays, $timeOfDay, $pagesPerDay, $title, $author, $pageCount)", dictionary);

        return DB.LastInsertedRowId();
    }

    public BookPlan? FindPlan(int id)
    {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT userid, deadline, weekdays, timeOfDay, pagesPerDay, title, author, pageCount, pagesRead FROM plans WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

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
        int pagesRead = reader.GetInt32(8);
        List<ReadingSession> sessions = ReadingSessions.GetAll(id);

        return new BookPlan(this, id, userid, deadline, weekdays, timeOfDay, pagesPerDay, title, author, pageCount, pagesRead, sessions);
    }

    public List<int> FindPlanByUser(int userId)
    {
        var ids = new List<int>();

        IEnumerable<SqliteDataReader> readers = DB.Execute(@"SELECT id FROM plans WHERE userId = $userId", new Dictionary<string, dynamic> { { "$userId", userId } });

        foreach (var reader in readers)
        {
            var id = reader.GetInt32(0);
            ids.Add(id);
        }
        if (ids.Count == 0)
        {
            return null;
        }
        return ids;
    }

    public bool UpdatePlan(int id, string deadLine, int weekdays, string timeOfDay, string title, string author, int pageCount)
    {
        if (pageCount < 0)
            throw new ArgumentException("Page count must be greater than zero");
        if (title.Length == 0)
            throw new ArgumentException("where title");
        if (author.Length == 0)
            throw new ArgumentException("where author");

        var parameters = new Dictionary<string, dynamic> {
            { "$id", id },
            { "$deadline", deadLine },
            { "$weekdays", weekdays },
            { "$timeOfDay", timeOfDay },
            { "$title", title },
            { "$author", author },
            { "$pageCount", pageCount }
        };

        DB.ExecuteNonQuery(@"UPDATE PLANS SET deadline = $deadline, weekdays = $weekdays, timeOfDay = $timeOfDay, title = $title, author = $author, pageCount = $pageCount WHERE id = $id", parameters);

        return true;
    }

    public List<ReadingSession> UpdateReadingSessions(int id, DateTime now)
    {
        BookPlan plan = FindPlan(id)!;

        List<ReadingSession> sessions = plan.GenerateReadingSessions(now);

        ReadingSessions.Invalidate(plan.Id, now);

        foreach (ReadingSession session in sessions)
        {
            ReadingSessions.Add(plan.Id, session);
        }

        return ReadingSessions.GetAll(plan.Id);
    }

    public void UpdatePagesRead(int id, int pagesRead)
    {
        if (pagesRead < 0)
            throw new ArgumentException("pagesRead cannot be negative");

        var parameters = new Dictionary<string, dynamic>
        {
            { "$id", id },
            { "$pagesRead", pagesRead }
        };

        DB.ExecuteNonQuery(@"UPDATE plans SET pagesRead = $pagesRead WHERE id = $id", parameters);
    }

    public void UpdatePagesPerDay(int id, int pagesPerDay)
    {
        if (pagesPerDay < 0)
            throw new ArgumentException("pagesRead cannot be negative");

        var parameters = new Dictionary<string, dynamic>
        {
            { "$id", id },
            { "$pagesPerDay", pagesPerDay }
        };

        DB.ExecuteNonQuery(@"UPDATE plans SET pagesPerDay = $pagesPerDay WHERE id = $id", parameters);
    }

    public bool DeletePlan(int id)
    {
        if (id < 0 || FindPlan(id) == null)
            throw new ArgumentException("invalid plan id");
        ReadingSessions.Delete(id);
        DB.ExecuteNonQuery(@"DELETE FROM plans WHERE id=$id ", new Dictionary<string, dynamic> { { "$id", id } });

        return true;
    }
}
