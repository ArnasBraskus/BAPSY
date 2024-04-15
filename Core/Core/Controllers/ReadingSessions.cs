public class ReadingSessions {
    private Database DB;

    public ReadingSessions(Database db) {
        DB = db;
    }

    public void Add(int planId, ReadingSession ev) {
        if (ev.Date.Length == 0)
            throw new ArgumentException("Date is empty");

        if (ev.Goal <= 0)
            throw new ArgumentException("Goal is not positive");

        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId},
            {"$date", ev.Date},
            {"$goal", ev.Goal}
        };

        DB.ExecuteNonQuery("INSERT INTO readingsessions (planId, date, goal) VALUES ($planId, $date, $goal)", parameters);
    }

    public void Delete(int planId) {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId}
        };

        DB.ExecuteNonQuery("DELETE FROM readingsessions WHERE planId = $planId", parameters);
    }

    public void Invalidate(int planId, DateTime dateAfter) {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId},
            {"$dateAfter", dateAfter.ToString("yyyy-MM-dd")}
        };

        DB.ExecuteNonQuery("DELETE FROM readingsessions WHERE planId = $planId AND date >= $dateAfter", parameters);
    }

    public ReadingSession Get(int id) {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$id", id}
        };

        var reader = DB.ExecuteSingle("SELECT date, goal, actual FROM readingsessions WHERE id = $id", parameters);

        if (reader == null)
            throw new KeyNotFoundException("Reading session not found");

        string date = reader.GetString(0);
        int goal = reader.GetInt32(1);
        int actual = reader.GetInt32(2);

        return new ReadingSession(this, id, date, goal, actual);
    }


    public List<ReadingSession> GetAll(int planId) {
        var parameters = new Dictionary<string, dynamic>
        {
            {"$planId", planId}
        };

        var sessions = new List<ReadingSession>();

        foreach (var ev in DB.Execute("SELECT id, date, goal, actual FROM readingsessions WHERE planId = $planId", parameters)) {
            int id = ev.GetInt32(0);
            string date = ev.GetString(1);
            int goal = ev.GetInt32(2);
            int actual = ev.GetInt32(3);

            sessions.Add(new ReadingSession(this, id, date, goal, actual));
        }

        return sessions;
    }

    public void SetActual(int id, int actual) {
        if (actual < 0)
            throw new ArgumentException("Actual is negative.");

        var parameters = new Dictionary<string, dynamic>
        {
            {"$id", id},
            {"$actual", actual}
        };

        DB.ExecuteNonQuery("UPDATE readingsessions SET actual = $actual WHERE id = $id", parameters);
    }
}
