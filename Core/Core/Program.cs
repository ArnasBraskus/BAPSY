Config conf = Config.Read("config.json");

bool dbExists = File.Exists(conf.DatabasePath);

Database db = new Database(conf.DatabasePath);

if (!db.Open())
    throw new InvalidDataException($"failed to open database {conf.DatabasePath}");

if (!dbExists) {
    db.Create(DatabaseSchema.Schema);
}

Users users = new Users(db);
Plans plans = new Plans(db);

var builder = WebApplication.CreateBuilder(args);

Auth auth = new Auth(conf.JwtSecretKey, conf.JwtIssuer, conf.GoogleApiClientId);

auth.Add(builder.Services);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Users", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

ApiBase[] apis = new ApiBase[] {
    new AuthApi(auth, users),
    new UserApi(users),
    new BookPlanApi(users, plans)
};

foreach (var api in apis) {
    api.Map(app);
}

app.Run();
