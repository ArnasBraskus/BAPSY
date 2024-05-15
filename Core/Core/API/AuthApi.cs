namespace Core;

public class AuthApi : ApiBase
{
    private readonly AuthUtils Auth;
    private readonly Users Users;

    public AuthApi(AuthUtils auth, Users users) : base(users)
    {
        Auth = auth;
        Users = users;
    }

    private sealed class GoogleAuthRequest
    {
        public required string JwtToken { get; set; } = null!;
    };

    internal class GoogleAuthResponse
    {
        public string Token { get; set; } = null!;
        public double Validity { get; set; }
    };

    public async Task<IResult> PostAuthGoogle(HttpRequest request)
    {
        var req = await ReadJson<GoogleAuthRequest>(request).ConfigureAwait(false);

        string email = string.Empty;
        string name = string.Empty;

        if (!Auth.ValidateGoogleJWT(req.JwtToken, ref email, ref name))
            return Results.BadRequest(new ErrorResponse { Error = "jwttoken is invalid." });

        if (!Users.UserExists(email))
        {
            Users.AddUser(email, name);
        }
        else
        {
            User user = Users.FindUser(email);

            if (user.Name != name)
            {
                user.Name = name;
            }
        }

        var validity = new TimeSpan(1, 0, 0, 0);

        string jwt = Auth.GenerateJWT(email, validity);

        return Results.Ok(new GoogleAuthResponse { Token = jwt, Validity = validity.TotalSeconds });
    }

    public override void Map(WebApplication app)
    {
        app.MapPost("/auth/google", PostAuthGoogle);
    }
}
