namespace Core;

public class ReadingEvent
{
    public DateTime Date { get; }
    public int PageStart { get; }
    public int PagesToRead { get; }
    public int PageEnd { get; }

    public ReadingEventMetadata Metadata;

    public ReadingEvent(DateTime date, int pageStart, int pagesToRead, ReadingEventMetadata meta)
    {
        Date = date;
        PagesToRead = pagesToRead;
        PageStart = pageStart;
        PageEnd = PageStart + PagesToRead - 1;
        Metadata = meta;
    }

    public override bool Equals(object obj)
    {
        ReadingEvent other = obj as ReadingEvent;

        return Date == other.Date && PageStart == other.PageStart && PagesToRead == other.PagesToRead;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
