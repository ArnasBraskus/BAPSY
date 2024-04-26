using System.Linq;

public class CalendarApi : ApiBase
{
    private Users Users;
    private Plans Plans;
    private DateTimeProvider DateTimeProvider;

    public CalendarApi(Users users, Plans plans, DateTimeProvider dateTimeProvider) : base(users)
    {
        Users = users;
        Plans = plans;
        DateTimeProvider = dateTimeProvider;
    }

    public CalendarApi(Users users, Plans plans) : this(users, plans, new DateTimeProvider()) {}

    public class GetCalendarEventsResponse
    {
        public class ReadingEvent
        {
            public string Date { get; set; } = null!;

            public string BookTitle { get; set; } = null!;
            public string BookAuthor { get; set; } = null!;

            public int PageStart { get; set; }
            public int PageEnd { get; set; }
            public int PagesToRead { get; set; }
        }

        public List<ReadingEvent> Events { get; set; } = null!;
    }

    public IResult GetCalendarEvents(HttpContext context)
    {
        User user = GetUser(context);

        ReadingCalendar calendar = new ReadingCalendar();

        foreach (int id in Plans.FindPlanByUser(user.Id)) {
            BookPlan? plan = Plans.FindPlan(id);

            if (plan is null)
                return Results.BadRequest(new ErrorResponse { Error = "Plan not found" });

            calendar.Add(plan);
        }

        return Results.Ok(new GetCalendarEventsResponse {
            Events = calendar.Events.Select(x => new GetCalendarEventsResponse.ReadingEvent {
                Date = x.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                BookTitle = x.Metadata.BookTitle,
                BookAuthor = x.Metadata.BookAuthor,
                PageStart = x.PageStart,
                PageEnd = x.PageEnd,
                PagesToRead = x.PagesToRead
            }).ToList()
        });
    }

    private string GenerateCalendarToken(User user)
    {
        return Auth.GenerateHash(user, $"CAL-{user.Id}");
    }

    public class GetCalendarTokenResponse
    {
        public string Token { get; set; } = null!;
    }

    public IResult GetCalendarToken(HttpContext context)
    {
        User user = GetUser(context);

        return Results.Ok(new GetCalendarTokenResponse {
            Token = GenerateCalendarToken(user)
        });
    }

    public IResult GetExportCalendar(HttpContext context, int userId, string t)
    {
        try {
            User user = Users.FindUser(userId);

            if (t != Auth.GenerateHash(user, $"CAL-{userId}"))
                return Results.BadRequest(new ErrorResponse { Error = "Invalid token" });

            ReadingCalendar calendar = new ReadingCalendar();

            foreach (int id in Plans.FindPlanByUser(user.Id)) {
                BookPlan? plan = Plans.FindPlan(id);

                if (plan is null)
                    return Results.BadRequest(new ErrorResponse { Error = "Plan not found" });

                calendar.Add(plan);
            }

            byte[] bytes = CalendarWriter.Serialize(calendar);

            return Results.Bytes(bytes, "text/calendar");
        }
        catch (KeyNotFoundException) {
            return Results.BadRequest(new ErrorResponse { Error = "Calendar not found" });
        }
    }

    public override void Map(WebApplication app)
    {
        app.MapGet("/calendar/events", GetCalendarEvents).RequireAuthorization("Users");
        app.MapGet("/calendar/token", GetCalendarToken).RequireAuthorization("Users");
        app.MapGet("/calendar/{userId}/export", GetExportCalendar);
    }
}
