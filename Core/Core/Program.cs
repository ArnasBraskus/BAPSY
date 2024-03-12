using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

Config conf = Config.Read("config.json");

Database db = new Database(conf.DatabasePath);

if (!db.Open())
    throw new InvalidDataException($"failed to open database {conf.DatabasePath}");

Users users = new Users(db);

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

Auth auth = new Auth(users, conf.JwtSecretKey, conf.JwtIssuer, conf.GoogleApiClientId);

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = auth.GetIssuer(),
        ValidAudience = auth.GetAudience(),
        IssuerSigningKey = auth.GetSigningKey(),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Users", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Authenticated.").RequireAuthorization("Users");
app.MapGet("/noauth", () => "No authentication.");

app.MapGet("/profile", (HttpContext context) =>
{
    var email = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

    if (email == null)
        return Results.BadRequest(new { Error = "Authentication error." });

    User? user = users.FindUser(email.Value);

    if (user == null)
        return Results.BadRequest(new { Error = "User profile not found." });

    return Results.Ok(new { Email = user.Email, Name = user.Name });
}).RequireAuthorization("Users");

auth.Map(app);

app.Run();
