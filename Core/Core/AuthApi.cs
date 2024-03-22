public class AuthApi : ApiBase {
    private Auth Auth;
    private Users Users;

    public AuthApi(Auth auth, Users users) : base(users) {
        Auth = auth;
        Users = users;
    }

    private class GoogleAuthRequest
    {
        public required string JwtToken { get; set; } = null!;
    };

    private async Task<IResult> PostAuthGoogle(HttpRequest request) {
        var req = await ReadJson<GoogleAuthRequest>(request);

        if (req is null)
            return BadJson;

        string email = string.Empty;
        string name = string.Empty;

        if (!Auth.ValidateGoogleJWT(req.JwtToken, ref email, ref name))
            return Results.BadRequest(new { Error = "jwttoken is invalid." });

        if (!Users.UserExists(email))
        {
            Users.AddUser(email, name);
        }

        var validity = new TimeSpan(1, 0, 0, 0);

        string jwt = Auth.GenerateJWT(email, validity);

        return Results.Ok(new { Token = jwt, Validity = validity.TotalSeconds });
    }

    public override void Map(WebApplication app)
    {
        app.MapPost("/auth/google", PostAuthGoogle);
    }
}
