using System.Net.Http.Json;
using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Users.LogInUser;

namespace Bookify.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTest
{
    protected readonly HttpClient HttpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected async Task<string> GetAccessToken()
    {
        HttpResponseMessage loginResponse = await HttpClient.PostAsJsonAsync(
            "api/v1/users/login",
            new LogInUserRequest(
                UserData.RegisterTestUserRequest.Email,
                UserData.RegisterTestUserRequest.Password));

        loginResponse.EnsureSuccessStatusCode();

        AccessTokenResponse accessTokenResponse =
            await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>()
            ?? throw new InvalidOperationException("Login did not return an access token.");

        return accessTokenResponse.AccessToken;
    }
}
