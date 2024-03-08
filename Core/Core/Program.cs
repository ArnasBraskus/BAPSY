using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

Config conf = Config.Read("config.json");

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenHandlers.Clear();
    options.TokenHandlers.Add(new GoogleTokenValidator(conf.GoogleApiClientId));
});


builder.Services.AddAuthorization(options => {
    options.AddPolicy("Users", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Authenticated.").RequireAuthorization("Users");
app.MapGet("/noauth", () => "No authentication.");

app.Run();
