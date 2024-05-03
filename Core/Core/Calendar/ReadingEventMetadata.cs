public class ReadingEventMetadata
{
    public string BookTitle { get; }
    public string BookAuthor { get; }

    public ReadingEventMetadata(string title, string author)
    {
        BookTitle = title;
        BookAuthor = author;
    }
}

