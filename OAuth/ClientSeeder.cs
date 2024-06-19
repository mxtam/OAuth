using OAuth.Data;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OAuth
{
        public class ClientsSeeder
        {
            private readonly IServiceProvider _serviceProvider;

            public ClientsSeeder(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }
            
            //Мето для додавання api scopes
            public async Task AddScopes()
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

                var apiScope = await manager.FindByNameAsync("api1");

                if (apiScope != null)
                {
                    await manager.DeleteAsync(apiScope);
                }

                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = "Api scope",
                    Name = "api1",
                    Resources =
                {
                    "resource_server_1"
                }
                });
            }
            //Додаємо web-client до списку клієнтів серверу авторизації(схожим чином можемо зробити метод, 
            //наприклад для react-client)
            public async Task AddWebClient()
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
                await context.Database.EnsureCreatedAsync();

                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

                var client = await manager.FindByClientIdAsync("web-client");
                if (client != null)
                {
                    await manager.DeleteAsync(client);
                }

                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "web-client",
                    ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "Swagger client application",
                    RedirectUris =
                {
                    new Uri("https://localhost:7002/swagger/oauth2-redirect.html")
                },
                    PostLogoutRedirectUris =
                {
                    new Uri("https://localhost:7002/resources")
                },
                    Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                   $"{Permissions.Prefixes.Scope}api1"
                },
                    //Requirements =
                    //{
                    //    Requirements.Features.ProofKeyForCodeExchange
                    //}
                });
            }
        }
}
