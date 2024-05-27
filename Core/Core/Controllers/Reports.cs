namespace Core;
using System;
using System.Collections.Generic;
using System.Globalization;

using Microsoft.Data.Sqlite;

public class ReportParams
{
    public int UserId { get; }
    public DateTime Date { get; }

    public ReportParams(int userId, DateTime date)
    {
        UserId = userId;
        Date = date;
    }
}

public class Reports
{
    private readonly Database DB;
    private readonly ReadingSessions ReadingSessions;
    private readonly Plans Plans;

    public Reports(Database db)
    {
        DB = db;
        Plans = new Plans(db);
        ReadingSessions = new ReadingSessions(db);
    }

    public int GenerateReportsIfNeeded(int userId)
    {
        DateTime currentDate = DateTime.UtcNow;
        DateTime latestReportDate;
        Report latestReport = FindLatestReportOfUser(userId);
        if (latestReport == null) 
        {

            var plan = Plans.FindFirstPlanOfUser(userId);
            latestReportDate = DateTime.Parse(plan.ReadingSessions.First().Date, CultureInfo.InvariantCulture);
        }
        else
        {
             latestReportDate = latestReport.Date;
        }

        while (latestReportDate.AddDays(14) <= currentDate)
        {
            DateTime reportStartDate = latestReportDate;
            DateTime reportEndDate = reportStartDate.AddDays(14);

            int newid = AddReport(userId, reportEndDate);
            
            latestReportDate = reportEndDate;
        }
        return DB.LastInsertedRowId();
    }
    public int AddReport(int userId, DateTime date)
    {
        int totalPages = 0;
        int completedPages = 0;
        int totalSessions = 0;
        int completedSessions = 0;

        DateTime startDate = date.AddDays(-14);
        DateTime endDate = date;

        IEnumerable<ReadingSession> sessions = ReadingSessions.GetByUserAndDateRange(userId, startDate, endDate);

        foreach (var session in sessions)
        {
            totalSessions++;
            totalPages += session.Goal;

            if (session.IsCompleted == 1)
            {
                completedSessions++;
                completedPages += session.Actual;
            }
        }

        int percentagePages = (totalPages > 0) ? (completedPages * 100) / totalPages : 0;
        int percentageSessions = (totalSessions > 0) ? (completedSessions * 100) / totalSessions : 0;

        var parameters = new Dictionary<string, dynamic> {
            { "$userId", userId },
            { "$totalPages", completedPages },
            { "$percentagePages", percentagePages },
            { "$totalSessions", completedSessions },
            { "$percentageSessions", percentageSessions },
            { "$date", date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) }
        };

        DB.ExecuteNonQuery(@"INSERT INTO Reports (userId, totalPages, percentagePages, totalSessions, percentageSessions, date) VALUES ($userId, $totalPages, $percentagePages, $totalSessions, $percentageSessions, $date)", parameters);

        return DB.LastInsertedRowId();
    }

    public Report? FindReport(int id)
    {
        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT userId, totalPages, percentagePages, totalSessions, percentageSessions, date FROM Reports WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

        if (reader == null)
            return null;

        return new Report
        (id, reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), 
        reader.GetInt32(3), reader.GetInt32(4), DateTime.Parse(reader.GetString(5), CultureInfo.InvariantCulture));
    }

    public ICollection<int> FindReportsByUser(int userId)
    {
        var ids = new List<int>();

        IEnumerable<SqliteDataReader> readers = DB.Execute(@"SELECT id FROM Reports WHERE userId = $userId", new Dictionary<string, dynamic> { { "$userId", userId } });

        foreach (var reader in readers)
        {
            ids.Add(reader.GetInt32(0));
        }

        return ids;
    }

    public Report FindLatestReportOfUser(int userId)
    {

        SqliteDataReader? reader = DB.ExecuteSingle(@"SELECT id
        FROM reports 
        WHERE userId = $userId
        ORDER BY date DESC
        LIMIT 1", new Dictionary<string, dynamic> { { "$userId", userId } });

        if (reader != null)
        {
           Report? report = FindReport(reader.GetInt32(0));
            return report;
        }
        else return null;

    }

    public bool DeleteReport(int id)
    {
        if (id < 0 || FindReport(id) == null)
            throw new ArgumentException("Invalid report id");

        DB.ExecuteNonQuery(@"DELETE FROM Reports WHERE id = $id", new Dictionary<string, dynamic> { { "$id", id } });

        return true;
    }
}


