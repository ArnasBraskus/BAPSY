using System.Globalization;

namespace Core;

public class ReadingSessions
{
    private readonly Database _dataBase;

    private readonly Users _users;

    public ReadingSessions(Database db)
    {
        _dataBase = db;
        _users = new Users(_dataBase);
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

        _dataBase.ExecuteNonQuery("INSERT INTO readingsessions (planId, date, goal, completed) VALUES ($planId, $date, $goal, $completed)", parameters);
    }

    public void Delete(int planId)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId}
        };

        _dataBase.ExecuteNonQuery("DELETE FROM readingsessions WHERE planId = $planId", parameters);
    }

    public void Invalidate(int planId, DateTime dateAfter)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId},
            {"$dateAfter", dateAfter.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}
        };

        _dataBase.ExecuteNonQuery("DELETE FROM readingsessions WHERE planId = $planId AND date >= $dateAfter AND completed = 0", parameters);
    }

    public ReadingSession Get(int id)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$id", id}
        };

        var reader = _dataBase.ExecuteSingle("SELECT planId, date, goal, actual, completed FROM readingsessions WHERE id = $id", parameters);

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

        foreach (var ev in _dataBase.Execute("SELECT id, date, goal, actual, completed FROM readingsessions WHERE planId = $planId", parameters))
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

        _dataBase.ExecuteNonQuery("UPDATE readingsessions SET completed = 1, actual = $actual WHERE id = $id", parameters);
    }

    public void UpdateCompletion(int id, int completion)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$id", id},
            {"$completed", completion}
        };
        _dataBase.ExecuteNonQuery("UPDATE readingsessions SET completed = $completed WHERE id = $id", parameters);

    }

    private int GetUserId(int id)
    {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$id", id}
        };

        return _dataBase.ExecuteScalar("SELECT p.userId FROM plans p LEFT JOIN readingsessions s ON s.planId = p.id");
    }

    public User GetUser(int id)
    {
        return _users.FindUser(GetUserId(id));
    }
}
