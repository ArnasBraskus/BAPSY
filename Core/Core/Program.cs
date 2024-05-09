using Core;

var builder = WebApplication.CreateBuilder(args);

Config conf = new Config();

builder.Configuration.Bind(conf);

Database db = new Database(conf.DatabaseConnectionString);

db.Open();
db.CreateIfEmpty(DatabaseSchema.Schema);

Users users = new Users(db);
Plans plans = new Plans(db);
ReadingSessions sessions = new ReadingSessions(db);

Auth auth = new Auth(conf.JwtSecretKey, conf.JwtIssuer, new GoogleTokenValidator(conf.GoogleApiClientId));

auth.Add(builder.Services);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Users", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler("/error");

app.Map("/error", ApiBase.ErrorPage);

ApiBase[] apis = new ApiBase[] {
    new AuthApi(auth, users),
    new UserApi(users),
    new CalendarApi(users, plans),
    new BookPlanApi(users, plans, sessions),
    new SessionApi(users, plans, sessions)
};

foreach (var api in apis)
{
    api.Map(app);
}

app.Run();

public partial class Program { }
