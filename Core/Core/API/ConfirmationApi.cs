﻿namespace Core;

using System.Numerics;

public class ConfirmationApi : ApiBase
{
    private readonly Plans Plans;
    private readonly ReadingSessions Sessions;

    public ConfirmationApi(Users users, Plans plans, ReadingSessions sessions) : base(users)
    {
        Plans = plans;
        Sessions = sessions;
    }
    internal class MarkSessionCompletedRequestJson
    {
        public required int PlanId { get; set; }
        public required int SessionId { get; set; }
    };

    internal class MarkSessionNotCompletedRequestJson
    {
        public required int PlanId { get; set; }
        public required int SessionId { get; set; }
    };



    public async Task<IResult> PostMarkSessionCompleted(HttpContext context)
    {
        var data = await ReadJson<MarkSessionCompletedRequestJson>(context.Request).ConfigureAwait(false);

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

        return Results.Ok(new { });
    }

    public async Task<IResult> PostMarkSessionNotCompleted(HttpContext context)
    {
        var data = await ReadJson<MarkSessionNotCompletedRequestJson>(context.Request).ConfigureAwait(false);

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

        return Results.Ok(new { });
    }

    public override void Map(WebApplication app)
    {
        app.MapPost("/api/mark-session-completed", (Delegate)PostMarkSessionCompleted).RequireAuthorization("Users");
        app.MapPost("/api/mark-session-not-completed", (Delegate)PostMarkSessionNotCompleted).RequireAuthorization("Users");
    }

}
