using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using static OpenIddict.Abstractions.OpenIddictConstants;
using OAuth.Data;
using OAuth.Services;
using OAuth;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("AuthConnectionString");
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
        //Встановлюємо наші ендпоінти для автризації
       options.SetAuthorizationEndpointUris("connect/authorize")
                .SetLogoutEndpointUris("connect/logout")
                .SetTokenEndpointUris("connect/token")
                /*.SetUserinfoEndpointUris("connect/userinfo")*/;

        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

        //Включаємо підтрику Authorization Code Flow+PKCE
        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        //Додаємо ключ який необхідний при підключенні серверу ресурсів
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
//Додаємо сервіс авторизації
builder.Services.AddTransient<AuthorizationService>();

builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(c =>
    {
        c.LoginPath = "/Authenticate";
    });

//Додаєм сервіс для заповнення клієнтів серверу авторизації
builder.Services.AddTransient<ClientsSeeder>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        //Додаємо ресурсний сервер до CORS
        policy.WithOrigins("https://localhost:7002")
            .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ClientsSeeder>();
    seeder.AddWebClient().GetAwaiter().GetResult();
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
