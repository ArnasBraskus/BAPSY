namespace Core;

using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net;
using System.Text;

public static class CalendarWriter
{
    private const string TimeZone = "Europe/Vilnius";

    public static byte[] Serialize(ReadingCalendar calendar)
    {
        Calendar icalendar = new Calendar();

        icalendar.AddTimeZone(new VTimeZone(TimeZone));

        foreach (var calEvent in calendar.Events)
        {
            icalendar.Events.Add(new CalendarEvent
            {
                Summary = $"Book Reading: {calEvent.Metadata.BookTitle} by {calEvent.Metadata.BookAuthor}",
                Description = $"Today's goal: {calEvent.PagesToRead} pages ({calEvent.PageStart}-{calEvent.PageEnd})\r\n{Program.Config.UrlBase}/confirmation/{calEvent.SessionId}?t={calEvent.MarkToken}",
                Start = new CalDateTime(calEvent.Date, TimeZone)
            });
        }

        var serializer = new CalendarSerializer();
        var serializedCalendar = serializer.SerializeToString(icalendar);

        return Encoding.ASCII.GetBytes(serializedCalendar);
    }
}
