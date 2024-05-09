namespace Core;

public class UserApi : ApiBase
{
    public UserApi(Users users) : base(users)
    {
    }

    public class UserProfileResponse
    {
        public required string Email { get; set; } = null!;
        public required string Name { get; set; } = null!;
    }

    public IResult GetUserProfile(HttpContext context)
    {
        User user = GetUser(context);

        return Results.Ok(new UserProfileResponse { Email = user.Email, Name = user.Name });
    }

    public override void Map(WebApplication app)
    {
        app.MapGet("/user/profile", GetUserProfile).RequireAuthorization("Users");
    }
}
