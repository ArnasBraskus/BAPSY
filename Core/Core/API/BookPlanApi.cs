namespace Core;

public class BookPlanApi : ApiBase
{
    private readonly Plans Plans;
    private readonly DateTimeProvider DateTimeProvider;
    private const string PlanNotFound = "Plan not found.";    
    private const string AuthorizationPolicyName = "Users";

    public BookPlanApi(Users users, Plans plans, DateTimeProvider dateTimeProvider) : base(users)
    {
        Plans = plans;
        DateTimeProvider = dateTimeProvider;
    }

    public BookPlanApi(Users users, Plans plans) : this(users, plans, new DateTimeProvider())
    {
    }

    internal class ListBookPlansResponse
    {
        public List<int> Ids { get; }
		public ListBookPlansResponse(List<int> ids)
		{
			Ids = ids;
		}
	}

    public IResult ListBookPlans(HttpContext context)
    {
        User user = GetUser(context);

        List<int> ids = Plans.FindPlanByUser(user.Id);

		return Results.Ok(new ListBookPlansResponse(ids));
	}

    internal class AddBookPlanRequest
    {
        public required string Title { get; set; } = null!;
        public required string Author { get; set; } = null!;
        public required int Pages { get; set; }
        public required string Deadline { get; set; } = null!;
        public required bool[] Weekdays { get; set; } = null!;
        public required string TimeOfDay { get; set; } = null!;
    }


    public async Task<IResult> PostAddBookPlan(HttpContext context)
    {
        User user = GetUser(context);

        var req = await ReadJson<AddBookPlanRequest>(context.Request).ConfigureAwait(false);

        try
        {
            PlanParams planParams = new PlanParams(user, req.Deadline, Weekdays.ToBitField(req.Weekdays), req.TimeOfDay, 0, req.Title, req.Author, req.Pages);

            int id = Plans.AddPlan(planParams);

            Plans.UpdateReadingSessions(id, DateTimeProvider.Now);
        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(new ErrorResponse { Error = exception.Message });
        }

        return Results.Ok(new { });
    }

    internal class GetBookPlanResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public int Pages { get; set; }
        public int PagesRead { get; set; }
        public string Deadline { get; set; } = null!;
        public bool[] Weekdays { get; set; } = null!;
        public string TimeOfDay { get; set; } = null!;
        public List<ReadingSession> Sessions { get; set; } = null!;
    }

    public IResult GetBookPlan(HttpContext context, int id)
    {
        User user = GetUser(context);

        BookPlan? plan = Plans.FindPlan(id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = PlanNotFound });

        return Results.Ok(new GetBookPlanResponse
        {
            Id = id,
            Author = plan.Author,
            Title = plan.Title,
            Pages = plan.PageCount,
            PagesRead = plan.PagesRead,
            Deadline = plan.DeadLine,
            Weekdays = Weekdays.FromBitField(plan.DayOfWeek),
            TimeOfDay = plan.timeOfDay,
            Sessions = plan.ReadingSessions
        });
    }

    internal class RemoveBookPlanRequest
    {
        public required int Id { get; set; }
    }

   

    public async Task<IResult> PostRemoveBookPlan(HttpContext context)
    {
        User user = GetUser(context);

        var req = await ReadJson<RemoveBookPlanRequest>(context.Request).ConfigureAwait(false);

        BookPlan? plan = Plans.FindPlan(req.Id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = PlanNotFound });

        Plans.DeletePlan(plan.Id);

        return Results.Ok(new {});
    }

    private sealed class EditBookPlanRequest
    {
        public required int Id { get; set; }
        public required string Title { get; set; } = null!;
        public required string Author { get; set; } = null!;
        public required int Pages { get; set; }
        public required string Deadline { get; set; } = null!;
        public required bool[] Weekdays { get; set; } = null!;
        public required string TimeOfDay { get; set; } = null!;
    };


    public async Task<IResult> PostEditBookPlan(HttpContext context)
    {
        User user = GetUser(context);

        var data = await ReadJson<EditBookPlanRequest>(context.Request).ConfigureAwait(false);

        BookPlan? plan = Plans.FindPlan(data.Id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = PlanNotFound });

        try
        {
            Plans.UpdatePlan(plan.Id, data.Deadline, Weekdays.ToBitField(data.Weekdays), data.TimeOfDay, data.Title, data.Author, data.Pages);
            Plans.UpdateReadingSessions(plan.Id, DateTimeProvider.Now);

        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(new ErrorResponse { Error = exception.Message });
        }

        return Results.Ok(new{ });
    }

	internal class UpdateAdditionalPagesReadRequest
	{
		public required int PlanId { get; set; }
		public required int AdditionalPagesRead { get; set; }
	}

	internal class UpdateAdditionalPagesReadResponse
	{
		public required string Message { get; set; }
	}
	public async Task<IResult> PostAdditionalPagesRead(HttpContext context)
	{
		try
		{
			User user = GetUser(context);
			var data = await ReadJson<UpdateAdditionalPagesReadRequest>(context.Request).ConfigureAwait(false);

			BookPlan? plan = Plans.FindPlan(data.PlanId);

			if (plan == null || plan.UserId != user.Id)
				return Results.BadRequest(new ErrorResponse { Error = PlanNotFound });

			plan.AdditionalPagesRead(data.AdditionalPagesRead);

			return Results.Ok(new UpdateAdditionalPagesReadResponse { Message = "Actual pages read updated successfully." });
		}
		catch (ArgumentException exception)
		{
			return Results.BadRequest(new ErrorResponse { Error = exception.Message });
		}
	}
    
	public override void Map(WebApplication app)
    {
        app.MapGet("/bookplan/list", ListBookPlans).RequireAuthorization(AuthorizationPolicyName);
        app.MapGet("/bookplan/get/{id}", GetBookPlan).RequireAuthorization(AuthorizationPolicyName);
        app.MapPost("/bookplan/add", (Delegate)PostAddBookPlan).RequireAuthorization(AuthorizationPolicyName);
        app.MapPost("/bookplan/remove", (Delegate)PostRemoveBookPlan).RequireAuthorization(AuthorizationPolicyName);
        app.MapPost("/bookplan/edit", (Delegate)PostEditBookPlan).RequireAuthorization(AuthorizationPolicyName);
		app.MapPost("/bookplan/additionalPages", (Delegate)PostAdditionalPagesRead).RequireAuthorization(AuthorizationPolicyName);
	}
}
