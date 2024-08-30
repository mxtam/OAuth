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
                    Resources ={ "resource_server_1" }
                });

                // Adding 'openid' scope
                var openidScope = await manager.FindByNameAsync("openid");
                if (openidScope != null)
                {
                    await manager.DeleteAsync(openidScope);
                }

                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = "OpenID Connect Scope",
                    Name = "openid",
                    Resources = { "resource_server_1" }
                });

                // Adding 'profile' scope
                var profileScope = await manager.FindByNameAsync("profile");
                if (profileScope != null)
                {
                    await manager.DeleteAsync(profileScope);
                }

                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = "Profile Scope",
                    Name = "profile",
                    Resources = { "resource_server_1" }
                });

                // Adding 'email' scope
                var emailScope = await manager.FindByNameAsync("email");
                if (emailScope != null)
                {
                    await manager.DeleteAsync(emailScope);
                }

                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = "Email Scope",
                    Name = "email",
                    Resources = { "resource_server_1" }
                });

                var offlineAccessScope = await manager.FindByNameAsync("offline_access");
                if (offlineAccessScope != null)
                {
                    await manager.DeleteAsync(offlineAccessScope);
                }

                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = "Offline Access (Refresh Token) Scope",
                    Name = "offline_access",
                    Resources = { "resource_server_1" }
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
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                   $"{Permissions.Prefixes.Scope}api1"
                },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }

            //Додаємо react-client до списку клієнтів серверу авторизації
            public async Task AddReactClient()
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
                await context.Database.EnsureCreatedAsync();

                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

                var client = await manager.FindByClientIdAsync("react-client");
                if (client != null)
                {
                    await manager.DeleteAsync(client);
                }

                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "react-client",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "React client application",
                    RedirectUris =
                    {
                        new Uri("http://localhost:3000/oauth/callback")
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri("http://localhost:3000/")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                       $"{Permissions.Prefixes.Scope}api1"
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }
            
            //Додаємо oidc-debugger до списку клієнтів серверу авторизації
            public async Task AddOidcDebuggerClient()
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
                await context.Database.EnsureCreatedAsync();

                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

                var client = await manager.FindByClientIdAsync("oidc-debugger");
                if (client != null)
                {
                    await manager.DeleteAsync(client);
                }

                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "oidc-debugger",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "Postman client application",
                    RedirectUris =
                    {
                        new Uri("https://oidcdebugger.com/debug")
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://oauth.pstmn.io/v1/callback")
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
                        $"{Permissions.Prefixes.Scope}api1",

                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }

        }
}
