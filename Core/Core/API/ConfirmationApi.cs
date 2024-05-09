namespace Core;

using System.Numerics;

public class ConfirmationApi : ApiBase
{
    private readonly Users Users;
    private readonly Plans Plans;
    private readonly ReadingSessions Sessions;


    public ConfirmationApi(Users users, Plans plans, ReadingSessions sessions) : base(users)
    {
        Users = users;
        Plans = plans;
        Sessions = sessions;
    }
    public class MarkSessionCompletedRequest
    {
        public int PlanId { get; set; }
        public int SessionId { get; set; }
    };

    public class MarkSessionNotCompletedRequest
    {
        public int PlanId { get; set; }
        public int SessionId { get; set; }
    };

    public class MarkSessionResponse
    {

    };

    public async Task<IResult> PostMarkSessionCompleted(HttpContext context)
    {
        User user = GetUser(context);

        var data = await ReadJson<MarkSessionCompletedRequest>(context.Request).ConfigureAwait(false);

        BookPlan? plan = Plans.FindPlan(data.PlanId);

        ReadingSession? session = Sessions.Get(data.SessionId);

        if (session == null || plan == null)
            return Results.BadRequest(new ErrorResponse { Error = "Session or plan not found." });

        try
        {
            Sessions.UpdateCompletion(session.Id, 1);
            plan.MarkReadingSession(session, session.Actual);

        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(new ErrorResponse { Error = e.Message });
        }

        return Results.Ok(new MarkSessionResponse { });
    }

    public async Task<IResult> PostMarkSessionNotCompleted(HttpContext context)
    {
        User user = GetUser(context);

        var data = await ReadJson<MarkSessionNotCompletedRequest>(context.Request).ConfigureAwait(false);

        ReadingSession? session = Sessions.Get(data.SessionId);
        BookPlan? plan = Plans.FindPlan(data.PlanId);

        if (session == null || plan == null)
            return Results.BadRequest(new ErrorResponse { Error = "Session or plan not found." });

        try
        {
            Sessions.UpdateCompletion(session.Id, 0);
            plan.MarkReadingSession(session, 0);

        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(new ErrorResponse { Error = e.Message });
        }

        return Results.Ok(new MarkSessionResponse { });
    }

    public override void Map(WebApplication app)
    {
        app.MapPost("/api/mark-session-completed", PostMarkSessionCompleted).RequireAuthorization("Users");
        app.MapPost("/api/mark-session-not-completed", PostMarkSessionNotCompleted).RequireAuthorization("Users");
    }

}
