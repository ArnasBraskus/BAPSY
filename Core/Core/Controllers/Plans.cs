namespace Core;

using System.Globalization;

using Microsoft.Data.Sqlite;


public struct PlanParams
{
    public User User { get; }
    public string Deadline { get; }
    public int Weekdays { get; }
    public string TimeOfDay { get; }
    public int PagesPerDay { get; }
    public string Title { get; }
    public string Author { get; }
    public string Cover { get; }
    public int PageCount { get; }

    public PlanParams(User user, string deadline, int weekdays, string timeOfDay, int pagesPerDay, string title, string author, string cover, int pageCount)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        if (title == null)
            throw new ArgumentNullException(nameof(title), "Title cannot be null");
        if (author == null)
            throw new ArgumentNullException(nameof(author), "Author cannot be null");
        if (pageCount < 0)
            throw new ArgumentException("Page count must be greater than zero");
        if (title.Length == 0)
            throw new ArgumentException("Title cannot be empty");
        if (author.Length == 0)
            throw new ArgumentException("Author cannot be empty");

        User = user;
        Deadline = deadline;
        Weekdays = weekdays;
        TimeOfDay = timeOfDay;
        PagesPerDay = pagesPerDay;
        Title = title;
        Author = author;
        Cover = cover;
        PageCount = pageCount;
    }
}

public class Plans
{
    private readonly Database DB;
    private readonly ReadingSessions ReadingSessions;

    public Plans(Database db)
    {
        DB = db;
        ReadingSessions = new ReadingSessions(db);
    }

    public int AddPlan(PlanParams planParams)
    {
        Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic> {
            { "$userid", planParams.User.Id },
            { "$deadline", planParams.Deadline },
            { "$weekdays", planParams.Weekdays },
            { "$timeOfDay", planParams.TimeOfDay },
            { "$pagesPerDay", planParams.PagesPerDay },
            { "$title", planParams.Title },
            { "$author", planParams.Author },
            { "$cover", planParams.Cover },
            { "$pageCount", planParams.PageCount }
        };

        DB.ExecuteNonQuery(@"INSERT INTO PLANS (userid, deadline, weekdays, timeOfDay, pagesPerDay, title, author, cover, pageCount) VALUES ($userid, $deadline, $weekdays, $timeOfDay, $pagesPerDay, $title, $author, $cover, $pageCount)", dictionary);

        return DB.LastInsertedRowId();
    }

    public BookPlan? FindPlan(int id)
    {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT userid, deadline, weekdays, timeOfDay, pagesPerDay, title, author, pageCount, pagesRead, finished, cover FROM plans WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

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
        int finished = reader.GetInt32(9);
        string cover = reader.GetString(10);
        ICollection<ReadingSession> sessions = ReadingSessions.GetAll(id);

        return new BookPlan(this, id, userid, deadline, weekdays, timeOfDay, pagesPerDay, title, author, cover, pageCount, pagesRead, sessions, finished);
    }

    public ICollection<int> FindPlanByUser(int userId)
    {
        var ids = new List<int>();

        IEnumerable<SqliteDataReader> readers = DB.Execute(@"SELECT id FROM plans WHERE userId = $userId", new Dictionary<string, dynamic> { { "$userId", userId } });

        foreach (var reader in readers)
        {
            var id = reader.GetInt32(0);
            ids.Add(id);
        }

        return ids;
    }
    public BookPlan? FindFirstPlanOfUser(int userId)
    {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT id FROM plans WHERE userId = $userId ORDER BY id ASC LIMIT 1", new Dictionary<string, dynamic> { { "$userId", userId } });

        if (reader == null)
        {
            Console.WriteLine("No plans found for user.");
            return null;
        }

        if (!reader.Read())
        {
            Console.WriteLine("Reader did not return any rows.");
            return null;
        }

        int planId = reader.GetInt32(0);
        BookPlan? bookplan = FindPlan(planId);

        if (bookplan == null)
        {
            Console.WriteLine($"Plan with ID {planId} not found.");
        }
        return bookplan;
    }


    public bool UpdatePlan(int id, string deadLine, int weekdays, string timeOfDay, string title, string author, string cover, int pageCount)
    {
        if (title == null)
            throw new ArgumentNullException(nameof(title), "Title cannot be null");
        if (author == null)
            throw new ArgumentNullException(nameof(author), "Author cannot be null");
        if (pageCount < 0)
            throw new ArgumentException("Page count must be greater than zero");
        if (title.Length == 0)
            throw new ArgumentException("Title cannot be empty");
        if (author.Length == 0)
            throw new ArgumentException("Author cannot be empty");

        var parameters = new Dictionary<string, dynamic> {
            { "$id", id },
            { "$deadline", deadLine },
            { "$weekdays", weekdays },
            { "$timeOfDay", timeOfDay },
            { "$title", title },
            { "$author", author },
            { "$cover", cover },
            { "$pageCount", pageCount }
        };

        DB.ExecuteNonQuery(@"UPDATE PLANS SET deadline = $deadline, weekdays = $weekdays, timeOfDay = $timeOfDay, title = $title, author = $author, cover = $cover, pageCount = $pageCount WHERE id = $id", parameters);

        return true;
    }

    public ICollection<ReadingSession> UpdateReadingSessions(int id, DateTime now)
    {
        BookPlan plan = FindPlan(id)!;

        ICollection<ReadingSession> sessions = plan.GenerateReadingSessions(now);

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
    public void UpdateFinished(int id)
    {
        if (id < 0 )
            throw new ArgumentException("invalid plan id");
        var BookPlan = FindPlan(id);

        if (BookPlan == null)
            throw new ArgumentException("invalid plan id");

        int completedSessions = 0; int totalSessions = 0;
        foreach (var session in BookPlan.ReadingSessions)
        {
            if (session.IsCompleted == 1)
                completedSessions++;
            totalSessions++;
        }
        if (completedSessions == totalSessions || DateTime.Parse(BookPlan.DeadLine, CultureInfo.InvariantCulture) > DateTime.Now.AddDays(1))
        {
            var parameters = new Dictionary<string, dynamic>
            {
                { "$id", id },
                { "$finished", 1 }
            };
            DB.ExecuteNonQuery(@"UPDATE plans SET finished = $finished WHERE id = $id", parameters);
        }
    }
}
