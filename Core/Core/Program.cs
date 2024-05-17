using Core;

var builder = WebApplication.CreateBuilder(args);

var conf = Program.Config;

builder.Configuration.Bind(conf);

Database db = new Database(Config.DatabaseConnectionString);

db.Open();
db.CreateIfEmpty(DatabaseSchema.Schema);

Users users = new Users(db);
Plans plans = new Plans(db);
ReadingSessions sessions = new ReadingSessions(db);

AuthUtils auth = new AuthUtils(conf.JwtSecretKey, conf.JwtIssuer, new GoogleTokenValidator(conf.GoogleApiClientId));

auth.Add(builder.Services);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Users", policy => policy.RequireAuthenticatedUser());

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler("/error");

app.Map("/error", ApiBase.ErrorPage);

ApiBase[] apis = new ApiBase[] {
    new AuthApi(auth, users),
    new UserApi(users),
    new CalendarApi(users, plans),
    new BookPlanApi(users, plans),
    new SessionApi(users, plans, sessions)
};

foreach (var api in apis)
{
    api.Map(app);
}

app.Run();

public static partial class Program {
    public static Config Config = new Config();
}
