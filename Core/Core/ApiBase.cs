using System.Text.Json;

public abstract class ApiBase {
    private Users Users;

    protected static readonly IResult BadAuth = Results.BadRequest(new { Error = "Authentication error." });
    protected static readonly IResult BadJson = Results.BadRequest(new { Error = "Malformed JSON." });

    public ApiBase(Users users) {
        Users = users;
    }

    protected User? CheckAuth(HttpContext context) {
        var email = Auth.GetNameIdentifier(context);

        if (email == null)
            return null;

        User? user = Users.FindUser(email);

        if (user == null)
            return null;

        return user;
    }

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions {
        PropertyNameCaseInsensitive = true
    };

    protected async Task<T?> ReadJson<T>(HttpRequest request) where T : class {
        var stream = await new StreamReader(request.Body).ReadToEndAsync();

        try {
            return JsonSerializer.Deserialize<T>(stream, JsonOptions);
        }
        catch (JsonException) {
            return null;
        }
    }

    public abstract void Map(WebApplication app);
}
