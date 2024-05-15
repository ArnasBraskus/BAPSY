namespace Core;

using System.Text.Json;

public abstract class ApiBase
{
    private readonly Users Users;

    protected class ErrorResponse
    {
        public string Error { get; set; } = null!;
    }

    protected ApiBase(Users users)
    {
        Users = users;
    }

    public static IResult ErrorPage(HttpContext context)
    {
        return Results.BadRequest(new ApiBase.ErrorResponse { Error = "Bad request" });
    }

    protected User GetUser(HttpContext context)
    {
        var email = AuthUtils.GetNameIdentifier(context);

        return Users.FindUser(email);
    }

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    protected async Task<T> ReadJson<T>(HttpRequest request) where T : class
    {
        using (var reader = new StreamReader(request.Body)) {
            var stream = await reader.ReadToEndAsync().ConfigureAwait(false);

            var data = JsonSerializer.Deserialize<T>(stream, JsonOptions);

            if (data is null)
                throw new JsonException();

            return data;
        }
    }

    public abstract void Map(WebApplication app);
}
