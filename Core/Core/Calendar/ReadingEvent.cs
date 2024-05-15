namespace Core;

public class ReadingEvent
{
    public DateTime Date { get; }
    public int PageStart { get; }
    public int PagesToRead { get; }
    public int PageEnd { get; }

    private ReadingEventMetadata _metadata;
    public ReadingEventMetadata Metadata
    {
        get => _metadata;
        set => _metadata = value;
    }

    public ReadingEvent(DateTime date, int pageStart, int pagesToRead, ReadingEventMetadata meta)
    {
        Date = date;
        PagesToRead = pagesToRead;
        PageStart = pageStart;
        PageEnd = PageStart + PagesToRead - 1;
        _metadata = meta;
    }

    public override bool Equals(object? obj)
    {
        
        if (obj is not ReadingEvent other)
        {
            return false;
        }

        return Date == other.Date && PageStart == other.PageStart && PagesToRead == other.PagesToRead;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + Date.GetHashCode();
        hash = hash * 23 + PageStart.GetHashCode();
        hash = hash * 23 + PagesToRead.GetHashCode();
        return hash;
    }
}
