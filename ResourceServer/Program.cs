using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenIddict()
                    .AddValidation(options =>
                    {
                        //Встановлюємо сервер авторизації
                        options.SetIssuer("https://localhost:7000/");
                        options.AddAudiences("resource_server_1");

                        //Ключ для реєстрації нашого ресурс серверу на сервері авторизації
                        options.AddEncryptionKey(new SymmetricSecurityKey(
                            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

                        options.UseSystemNetHttp();
                        options.UseAspNetCore();
                    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://localhost:7000/connect/authorize"),
                TokenUrl = new Uri("https://localhost:7000/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "api1", "resource server scope" }
                }
            },
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors();

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI(c => 
    {
        c.OAuthClientId("web-client");
        c.OAuthClientSecret("901564A5-E7FE-42CB-B10D-61EF6A8F3654");
        c.OAuthUsePkce();
    });


app.UseHttpsRedirection();

//app.UseRequestLocalization(new RequestLocalizationOptions
//{
//    ApplyCurrentCultureToResponseHeaders = true
//});

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
