public class UserApi : ApiBase {
    private Users Users;

    public UserApi(Users users) : base(users) {
        Users = users;
    }

    public IResult GetUserProfile(HttpContext context) {
        User? user = CheckAuth(context);

        if (user is null)
            return BadAuth;

        return Results.Ok(new { Email = user.Email, Name = user.Name });
    }

    public override void Map(WebApplication app) {
        app.MapGet("/user/profile", GetUserProfile).RequireAuthorization("Users");
    }
}
