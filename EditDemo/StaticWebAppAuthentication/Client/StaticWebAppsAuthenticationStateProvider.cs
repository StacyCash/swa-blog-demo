using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace StaticWebAppAuthentication.Client
{
    public class StaticWebAppsAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public StaticWebAppsAuthenticationStateProvider(IConfiguration config, HttpClient httpClient)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var clientPrincipal = await GetClientPrinciple();
                var claimsPrincipal = ClientPrincipleToClaimsPrinciple.GetClaimsFromClientClaimsPrincipal(clientPrincipal);
                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }

        private async Task<ClientPrincipal> GetClientPrinciple()
        {
            var authDataUrl = _config.GetValue("StaticWebAppsAuthentication:AuthenticationDataUrl", "/.auth/me");
            var data = await _http.GetFromJsonAsync<AuthenticationData>(authDataUrl);
            var clientPrincipal = data?.ClientPrincipal ?? new ClientPrincipal();
            return clientPrincipal;
        }
    }
}
