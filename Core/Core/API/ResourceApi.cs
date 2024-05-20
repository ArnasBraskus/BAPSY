namespace Core;

public class ResourceApi : ApiBase
{
    public ResourceApi(Users users) : base(users)
    {

    }

    internal class PostUploadImageResponse
    {
        public string Image { get; set; } = null!;
    }

    private static string GetImagePath(string image)
    {
        return Path.Combine(Program.Config.ResourcesPath, image);
    }

    public async Task<IResult> PostUploadImage(HttpRequest request)
    {
        var image = Guid.NewGuid().ToString("N");
        var path = GetImagePath(image);

        await request.Body.CopyToAsync(new FileStream(path, FileMode.Create));

        return Results.Ok(new PostUploadImageResponse { Image = image } );
    }

    public IResult GetImage(HttpRequest request, string image)
    {
        var path = GetImagePath(image);

        if (!Path.Exists(path))
            return Results.NotFound();

        var stream = new FileStream(path, FileMode.Open);

        return Results.Stream(stream, "application/octet-stream");
    }

    public override void Map(WebApplication app) {
        app.MapPost("/resources/upload_image", PostUploadImage).RequireAuthorization("Users");
        app.MapGet("/resources/image/{image}", GetImage);
    }
}
