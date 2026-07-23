using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("bookify-db")
                      .WithHostPort(5432)
                      .WithDataVolume()
                      .WithLifetime(ContainerLifetime.Persistent);

var bookifyDb = postgres.AddDatabase("bookify");

var redis = builder.AddRedis("bookify-redis");

var api = builder.AddProject<Bookify_Api>("bookify-api")
                 .WithExternalHttpEndpoints()
                 .WithReference(bookifyDb)
                 .WithReference(redis)
                 .WaitFor(postgres)
                 .WaitFor(redis);

if (builder.ExecutionContext.IsRunMode)
{
    var keycloak = builder.AddKeycloak("keycloak", 8080)
                          .WithExternalHttpEndpoints()
                          .WithDataVolume()
                          .WithRealmImport("../../.files");

    api.WithReference(keycloak).WaitFor(keycloak);
}
else if (builder.ExecutionContext.IsPublishMode)
{
    IResourceBuilder<ParameterResource> parameter = builder.AddParameter("KeycloakUrl");

    api.WithEnvironment("services__keycloak__https__0", parameter);
}

await builder.Build().RunAsync();
