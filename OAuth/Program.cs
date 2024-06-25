using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using static OpenIddict.Abstractions.OpenIddictConstants;
using OAuth.Data;
using OAuth.Services;
using OAuth;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("AuthConnectionString");
var testConnectionString = builder.Configuration.GetConnectionString("TestConnectionString");
// Add services to the container.
builder.Services.AddDbContext<AuthContext>
    (options => {
        options.UseSqlServer(connectionString);
        options.UseOpenIddict();
    }) ;
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
                .UseDbContext<AuthContext>();
    })
    .AddServer(options =>
    {
        //Set authorization endpoints
       options.SetAuthorizationEndpointUris("connect/authorize")
                .SetLogoutEndpointUris("connect/logout")
                .SetTokenEndpointUris("connect/token")
                /*.SetUserinfoEndpointUris("connect/userinfo")*/;

        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

        //Enable Authorization Code Flow+PKCE
        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        //Encryption key to connect our servers 
        options.AddEncryptionKey(new SymmetricSecurityKey(
                            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));


        options.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
                .EnableAuthorizationEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
                .EnableTokenEndpointPassthrough()
                .EnableUserinfoEndpointPassthrough();
    });
//Adding auth service
builder.Services.AddTransient<AuthorizationService>();

builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(c =>
    {
        c.LoginPath = "/Authenticate";
    });

//Adding client seeder service for our clients on auth server
builder.Services.AddTransient<ClientsSeeder>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        //Adding esource server to CORS
        policy.WithOrigins("https://localhost:7002")
            .AllowAnyHeader();

        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ClientsSeeder>();
    seeder.AddWebClient().GetAwaiter().GetResult();
    seeder.AddReactClient().GetAwaiter().GetResult();
    seeder.AddScopes().GetAwaiter().GetResult();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseRequestLocalization( new RequestLocalizationOptions 
{ 
    ApplyCurrentCultureToResponseHeaders = true
});

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
