using System.Globalization;

using System.Globalization;

namespace Core;

public class ReadingSessions
{
    private readonly Database DB;

    private readonly Users Users;

    public ReadingSessions(Database db)
    {
        DB = db;
        Users = new Users(DB);
    }

    public void Add(int planId, ReadingSession ev)
    {
        if (ev.Date.Length == 0)
            throw new ArgumentException("Date is empty");

        if (ev.Goal <= 0)
            throw new ArgumentException("Goal is not positive");

        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId},
            {"$date", ev.Date},
            {"$goal", ev.Goal},
            {"$completed", ev.IsCompleted},
        };

        DB.ExecuteNonQuery("INSERT INTO readingsessions (planId, date, goal, completed) VALUES ($planId, $date, $goal, $completed)", parameters);
    }

    public void Delete(int planId)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId}
        };

        DB.ExecuteNonQuery("DELETE FROM readingsessions WHERE planId = $planId", parameters);
    }

    public void Invalidate(int planId, DateTime dateAfter)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId},
            {"$dateAfter", dateAfter.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}
            {"$dateAfter", dateAfter.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}
        };

        DB.ExecuteNonQuery("DELETE FROM readingsessions WHERE planId = $planId AND date >= $dateAfter AND completed = 0", parameters);
    }

    public ReadingSession Get(int id)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$id", id}
        };

        var reader = DB.ExecuteSingle("SELECT planId, date, goal, actual, completed FROM readingsessions WHERE id = $id", parameters);

        if (reader == null)
            throw new KeyNotFoundException("Reading session not found");

        int planId = reader.GetInt32(0);
        string date = reader.GetString(1);
        int goal = reader.GetInt32(2);
        int actual = reader.GetInt32(3);
        int isCompleted = reader.GetInt32(4);

        return new ReadingSession(this, id, planId, date, goal, actual, isCompleted);
    }


    public List<ReadingSession> GetAll(int planId)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId}
        };

        var sessions = new List<ReadingSession>();

        foreach (var ev in DB.Execute("SELECT id, date, goal, actual, completed FROM readingsessions WHERE planId = $planId", parameters))
        {
            int id = ev.GetInt32(0);
            string date = ev.GetString(1);
            int goal = ev.GetInt32(2);
            int actual = ev.GetInt32(3);
            int isCompleted = ev.GetInt32(4);

            sessions.Add(new ReadingSession(this, id, planId, date, goal, actual, isCompleted));
        }

        return sessions;
    }

    public void SetActual(int id, int actual)
    {
        if (actual < 0)
            throw new ArgumentException("Actual is negative.");

        var parameters = new Dictionary<string, dynamic>
        {
            {"$id", id},
            {"$actual", actual}
        };

        DB.ExecuteNonQuery("UPDATE readingsessions SET completed = 1, actual = $actual WHERE id = $id", parameters);
    }

    public void UpdateCompletion(int id, int completion)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$id", id},
            {"$completed", completion}
        };
        DB.ExecuteNonQuery("UPDATE readingsessions SET completed = $completed WHERE id = $id", parameters);

    }

    private int GetUserId(int id)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$id", id}
        };

        return DB.ExecuteScalar("SELECT p.userId FROM plans p LEFT JOIN readingsessions s ON s.planId = p.id");
    }

    public User GetUser(int id)
    {
        return Users.FindUser(GetUserId(id));
    }
}
