using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

Config conf = Config.Read("config.json");

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

Auth auth = new Auth(conf.JwtSecretKey, conf.JwtIssuer, conf.GoogleApiClientId);

services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o => {
    o.TokenValidationParameters = new TokenValidationParameters() {
        ValidIssuer = auth.GetIssuer(),
        ValidAudience = auth.GetAudience(),
        IssuerSigningKey = auth.GetSigningKey(),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization(options => {
    options.AddPolicy("Users", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Authenticated.").RequireAuthorization("Users");
app.MapGet("/profile", (HttpContext context) => {
    var email = context.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
    return new { Email = email };
}).RequireAuthorization("Users");
app.MapGet("/noauth", () => "No authentication.");

auth.Map(app);

app.Run();
