using System.Text.Json;

public class AuthApi {
    private Auth Auth;
    private Users Users;

    public AuthApi(Auth auth, Users users) {
        Auth = auth;
        Users = users;
    }

    private class GoogleAuthRequest
    {
        public string JwtToken { get; set; } = null!;
    };

    private async Task<IResult> PostAuthGoogle(HttpRequest request) {
        var stream = await new StreamReader(request.Body).ReadToEndAsync();
        var req = JsonSerializer.Deserialize<GoogleAuthRequest>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (req == null || req.JwtToken == null)
            return Results.BadRequest(new { Error = "jwttoken is not specified." });

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

    public void Map(WebApplication app)
    {
        app.MapPost("/auth/google", PostAuthGoogle);
    }
}
