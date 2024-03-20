public class UserApi {
    private Users Users;

    public UserApi(Users users) {
        Users = users;
    }

    public IResult GetUserProfile(HttpContext context) {
        var email = Auth.GetNameIdentifier(context);

        if (email == null)
            return Results.BadRequest(new { Error = "Authentication error." });

        User? user = Users.FindUser(email);

        if (user == null)
            return Results.BadRequest(new { Error = "User profile not found." });

        return Results.Ok(new { Email = user.Email, Name = user.Name });
    }

    public void Map(WebApplication app) {
        app.MapGet("/user/profile", GetUserProfile).RequireAuthorization("Users");
    }
}
