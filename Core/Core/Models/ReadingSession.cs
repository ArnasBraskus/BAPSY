namespace Core;

public class ReadingSession
{
    private readonly ReadingSessions? ReadingSessions;
    public int Id { get; }
    public int PlanId { get; }
    public string Date { get; }
    public int Goal { get; }

    private int _Actual;

    public int IsCompleted { get; }
    public int Actual
    {
        get
        {
            return _Actual;
        }
        set
        {
            if (ReadingSessions is null)
                throw new InvalidOperationException("ReadingSession is not connected to database");

            ReadingSessions.SetActual(Id, value);

            _Actual = value;
        }
    }

    public ReadingSession(string date, int goal)
    {
        Date = date;
        Goal = goal;
    }

    public ReadingSession(ReadingSessions events, int id, int planId, string date, int goal, int actual, int isCompleted)
    {
        ReadingSessions = events;
        Id = id;
        PlanId = planId;
        Date = date;
        Goal = goal;
        _Actual = actual;
        IsCompleted = isCompleted;
    }

    public string GenerateToken()
    {
        if (ReadingSessions is null)
            throw new InvalidOperationException("ReadingSession is not connected to database");

        User user = ReadingSessions.GetUser(Id);

        return AuthUtils.GenerateHash(user, $"SESSION-{Id}");
    }
}
