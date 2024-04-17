public class CalendarApi : ApiBase
{
    private Plans Plans;
    private DateTimeProvider DateTimeProvider;

    public CalendarApi(Users users, Plans plans, DateTimeProvider dateTimeProvider) : base(users)
    {
        Plans = plans;
        DateTimeProvider = dateTimeProvider;
    }

    public CalendarApi(Users users, Plans plans) : this(users, plans, new DateTimeProvider()) {}

    public async Task<IResult> GetCalendarEvents(HttpContext context, int id)
    {
        User user = GetUser(context);

        BookPlan? plan = Plans.FindPlan(id);

        if (plan is null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found" });

        ReadingCalendar calendar = ReadingCalendar.Create(plan, DateTimeProvider.Now);

        return Results.Ok(calendar.Events);
    }

    public async Task<IResult> GetExportCalendar(HttpContext context, int id)
    {
        BookPlan? plan = Plans.FindPlan(id);

        if (plan is null)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found" });

        ReadingCalendar calendar = ReadingCalendar.Create(plan, DateTimeProvider.Now);

        byte[] bytes = CalendarWriter.Serialize(calendar);

        return Results.Bytes(bytes, "text/calendar");
    }

    public override void Map(WebApplication app)
    {
        app.MapGet("/calendar/{id}/events", GetCalendarEvents).RequireAuthorization("Users");
        app.MapGet("/calendar/{id}/export", GetExportCalendar);
    }
}
