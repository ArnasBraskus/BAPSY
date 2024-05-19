namespace Core;

public class Report
{
	public int Id { get; set; }
	public int UserId { get; set; }
	public int TotalPages { get; set; }
    public int PercentagePages { get; set; }    
   
	public int TotalSessions { get; set; }

    public int PercentageSessions {  get; set; }
	public DateTime Date { get; set; } = DateTime.UtcNow;

    public Report(int id, int userId, int totalPages, int percentagePages, int totalSessions, int percentageSessions, DateTime date)
    {
        Id = id;
        UserId = userId;
        TotalPages = totalPages;
        PercentagePages = percentagePages;
        TotalSessions = totalSessions;
        PercentageSessions = percentageSessions;
        Date = date;
    }
}