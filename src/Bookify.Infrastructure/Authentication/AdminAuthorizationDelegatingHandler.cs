using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bookify.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication;

internal sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly KeycloakAdminAuthService _keycloakAdminAuthService;

    public AdminAuthorizationDelegatingHandler(KeycloakAdminAuthService keycloakAdminAuthService)
    {
        _keycloakAdminAuthService = keycloakAdminAuthService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        AuthorizationToken authorizationToken = await _keycloakAdminAuthService.GetAuthorizationToken(
            cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            authorizationToken.AccessToken);

        HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        return httpResponseMessage;
    }
}
