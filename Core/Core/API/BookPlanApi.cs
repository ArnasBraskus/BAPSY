
public class BookPlanApi : ApiBase
{
    private Users Users;
    private Plans Plans;
    private ReadingSessions ReadingSessions;
    private DateTimeProvider DateTimeProvider;

    public BookPlanApi(Users users, Plans plans, ReadingSessions sessions, DateTimeProvider dateTimeProvider) : base(users)
    {
        Users = users;
        Plans = plans;
        ReadingSessions = sessions;
        DateTimeProvider = dateTimeProvider;
    }

    public BookPlanApi(Users users, Plans plans, ReadingSessions sessions) : this(users, plans, sessions, new DateTimeProvider())
    {
    }

    public class ListBookPlansResponse
    {
        public List<int> Ids { get; set; } = null!;
    }

    public IResult ListBookPlans(HttpContext context)
    {
        User user = GetUser(context);

        List<int> ids = Plans.FindPlanByUser(user.Id);

        return Results.Ok(new ListBookPlansResponse { Ids = ids });
    }

    public class AddBookPlanRequest
    {
        public required string Title { get; set; } = null!;
        public required string Author { get; set; } = null!;
        public required int Pages { get; set; }
        public required string Deadline { get; set; } = null!;
        public required bool[] Weekdays { get; set; } = null!;
        public required string TimeOfDay { get; set; } = null!;
    }

    public class AddBookPlanResponse
    {
    };

    public async Task<IResult> PostAddBookPlan(HttpContext context)
    {
        User user = GetUser(context);

        var req = await ReadJson<AddBookPlanRequest>(context.Request).ConfigureAwait(false);

        try
        {
            int id = Plans.AddPlan(user, req.Deadline, Weekdays.ToBitField(req.Weekdays), req.TimeOfDay, 0, req.Title, req.Author, req.Pages);

            Plans.UpdateReadingSessions(id, DateTimeProvider.Now);
        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(new ErrorResponse { Error = e.Message });
        }

        return Results.Ok(new AddBookPlanResponse { });
    }

    public class GetBookPlanResponse
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
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found." });

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

    public class RemoveBookPlanRequest
    {
        public required int Id { get; set; }
    }

    public class RemoveBookPlanResponse
    {

    }

    public async Task<IResult> PostRemoveBookPlan(HttpContext context)
    {
        User user = GetUser(context);

        var req = await ReadJson<RemoveBookPlanRequest>(context.Request).ConfigureAwait(false);

        BookPlan? plan = Plans.FindPlan(req.Id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found." });

        Plans.DeletePlan(plan.Id);

        return Results.Ok(new RemoveBookPlanResponse { });
    }

    private class EditBookPlanRequest
    {
        public required int Id { get; set; }
        public required string Title { get; set; } = null!;
        public required string Author { get; set; } = null!;
        public required int Pages { get; set; }
        public required string Deadline { get; set; } = null!;
        public required bool[] Weekdays { get; set; } = null!;
        public required string TimeOfDay { get; set; } = null!;
    };

    public class EditBookPlanResponse
    {

    };

    public async Task<IResult> PostEditBookPlan(HttpContext context)
    {
        User user = GetUser(context);

        var data = await ReadJson<EditBookPlanRequest>(context.Request).ConfigureAwait(false);

        BookPlan? plan = Plans.FindPlan(data.Id);

        if (plan == null || plan.UserId != user.Id)
            return Results.BadRequest(new ErrorResponse { Error = "Plan not found." });

        try
        {
            Plans.UpdatePlan(plan.Id, data.Deadline, Weekdays.ToBitField(data.Weekdays), data.TimeOfDay, data.Title, data.Author, data.Pages);
            Plans.UpdateReadingSessions(plan.Id, DateTimeProvider.Now);

        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(new ErrorResponse { Error = e.Message });
        }

        return Results.Ok(new EditBookPlanResponse { });
    }

	public class UpdateAdditionalPagesReadRequest
	{
		public int PlanId { get; set; }
		public int ActualPagesRead { get; set; }
	}

	public class UpdateAdditionalPagesReadResponse
	{
		public string Message { get; set; }
	}
	public async Task<IResult> PostAdditionalPagesRead(HttpContext context)
	{
		try
		{
			User user = GetUser(context);
			var data = await ReadJson<UpdateAdditionalPagesReadRequest>(context.Request).ConfigureAwait(false);

			BookPlan? plan = Plans.FindPlan(data.PlanId);

			if (plan == null || plan.UserId != user.Id)
				return Results.BadRequest(new ErrorResponse { Error = "Plan not found." });

			plan.AdditionalPagesRead(data.ActualPagesRead);

			return Results.Ok(new UpdateAdditionalPagesReadResponse { Message = "Actual pages read updated successfully." });
		}
		catch (ArgumentException e)
		{
			return Results.BadRequest(new ErrorResponse { Error = e.Message });
		}
	}

	public override void Map(WebApplication app)
    {
        app.MapGet("/bookplan/list", ListBookPlans).RequireAuthorization("Users");
        app.MapGet("/bookplan/get/{id}", GetBookPlan).RequireAuthorization("Users");
        app.MapPost("/bookplan/add", PostAddBookPlan).RequireAuthorization("Users");
        app.MapPost("/bookplan/remove", PostRemoveBookPlan).RequireAuthorization("Users");
        app.MapPost("/bookplan/edit", PostEditBookPlan).RequireAuthorization("Users");
		app.MapPost("/bookplan/additionalPages", PostAdditionalPagesRead).RequireAuthorization("Users");
	}
}
