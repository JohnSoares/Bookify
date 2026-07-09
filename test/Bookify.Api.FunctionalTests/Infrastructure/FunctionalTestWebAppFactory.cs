using System.Net;
using System.Net.Http.Json;
using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Abstractions.Data;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Bookify.Api.FunctionalTests.Infrastructure;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("bookify")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:latest").Build();

    private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder("quay.io/keycloak/keycloak:latest")
        .WithResourceMapping(
            new FileInfo(".files/bookify-realm-export.json"),
            new FileInfo("/opt/keycloak/data/import/realm.json"))
        .WithCommand("--import-realm")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureAppConfiguration(configurationBuilder =>
            configurationBuilder
                .AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false));

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            string connectionString = $"{_dbContainer.GetConnectionString()};Pooling=False";

            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention()
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

            services.RemoveAll<IDbConnectionFactory>();

            services.AddSingleton<IDbConnectionFactory>(_ =>
                new DbConnectionFacotry(new NpgsqlDataSourceBuilder(connectionString).Build()));

            services.Configure<RedisCacheOptions>(redisCacheOptions =>
                redisCacheOptions.Configuration = _redisContainer.GetConnectionString());

            string? keycloakAddress = _keycloakContainer.GetBaseAddress();

            services.Configure<KeycloakOptions>(o =>
            {
                o.AdminUrl = $"{keycloakAddress}admin/realms/bookify/";
                o.TokenUrl = $"{keycloakAddress}realms/bookify/protocol/openid-connect/token";
            });

            services.Configure<AuthenticationOptions>(o =>
            {
                o.Issuer = $"{keycloakAddress}realms/bookify/";
                o.MetadataUrl = $"{keycloakAddress}realms/bookify/.well-known/openid-configuration";
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _keycloakContainer.StartAsync();

        await InitializeTestUserAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeContainersAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            await base.DisposeAsync();
        }
        catch (TaskCanceledException)
        {
            // Docker cleanup can time out after the tests have already completed.
        }

        await DisposeContainersAsync();
    }

    private async Task InitializeTestUserAsync()
    {
        using HttpClient httpClient = CreateClient();

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "api/v1/users/register",
            UserData.RegisterTestUserRequest);

        if (response.StatusCode is not HttpStatusCode.OK)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                $"Failed to initialize test user. Status code: {response.StatusCode}. Response body: {responseBody}");
        }
    }

    private async Task DisposeContainersAsync()
    {
        await DisposeContainerAsync(_dbContainer);
        await DisposeContainerAsync(_redisContainer);
        await DisposeContainerAsync(_keycloakContainer);
    }

    private static async Task DisposeContainerAsync(IAsyncDisposable container)
    {
        try
        {
            await container.DisposeAsync();
        }
        catch (TaskCanceledException)
        {
            // Docker cleanup can time out after the tests have already completed.
        }
    }
}
