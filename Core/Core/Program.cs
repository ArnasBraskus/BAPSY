Config conf = Config.Read("config.json");

Database db = new Database(conf.DatabasePath);

if (!db.Open())
    throw new InvalidDataException($"failed to open database {conf.DatabasePath}");

Users users = new Users(db);

var builder = WebApplication.CreateBuilder(args);

Auth auth = new Auth(users, conf.JwtSecretKey, conf.JwtIssuer, conf.GoogleApiClientId);

auth.Add(builder.Services);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Users", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

AuthApi authApi = new AuthApi(auth, users);
UserApi userApi = new UserApi(users);

userApi.Map(app);
authApi.Map(app);

app.Run();
