using System.Text.Json;

public abstract class ApiBase {
    private Users Users;

    protected static readonly IResult BadAuth = Results.BadRequest(new ErrorResponse { Error = "Authentication error." });
    protected static readonly IResult BadJson = Results.BadRequest(new ErrorResponse { Error = "Malformed JSON." });

    public class ErrorResponse {
        public string Error { get; set; } = null!;
    }

    public ApiBase(Users users) {
        Users = users;
    }

    protected User? CheckAuth(HttpContext context) {
        var email = Auth.GetNameIdentifier(context);

        if (email == null)
            return null;

        return Users.FindUser(email);
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
