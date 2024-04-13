public class ReadingEvent
{
    public DateTime Date { get; }
    public int PageStart { get; }
    public int PagesToRead { get; }
    public int PageEnd { get; }

    public ReadingEvent(DateTime date, int pageStart, int pagesToRead)
    {
        Date = date;
        PagesToRead = pagesToRead;
        PageStart = pageStart;
        PageEnd = PageStart + PagesToRead - 1;
    }
}
