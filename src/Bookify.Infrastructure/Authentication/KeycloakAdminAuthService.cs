using System.Net.Http.Json;
using Bookify.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication;

internal sealed class KeycloakAdminAuthService
{
    private readonly KeycloakOptions _keycloakOptions;
    private readonly HttpClient _httpClient;

    public KeycloakAdminAuthService(IOptions<KeycloakOptions> keycloakOptions, HttpClient httpClient)
    {
        _keycloakOptions = keycloakOptions.Value;
        _httpClient = httpClient;
    }

    public async Task<AuthorizationToken> GetAuthorizationToken(CancellationToken cancellationToken)
    {
        var authorizationRequestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _keycloakOptions.AdminClientId),
            new("client_secret", _keycloakOptions.AdminClientSecret),
            new("scope", "openid email"),
            new("grant_type", "client_credentials")
        };

        var authorizationRequestContent = new FormUrlEncodedContent(authorizationRequestParameters);

        using var authorizationRequest = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(_keycloakOptions.TokenUrl))
        {
            Content = authorizationRequestContent
        };

        HttpResponseMessage authorizationResponse = await _httpClient.SendAsync(
            authorizationRequest,
            cancellationToken);

        authorizationResponse.EnsureSuccessStatusCode();

        return await authorizationResponse.Content.ReadFromJsonAsync<AuthorizationToken>(cancellationToken) ??
               throw new ApplicationException();
    }
}
