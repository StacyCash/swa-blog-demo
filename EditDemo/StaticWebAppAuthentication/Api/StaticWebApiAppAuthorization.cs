using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace StaticWebAppAuthentication.Api;

public static class StaticWebApiAppAuthorization
{
    public static ClientPrincipal ParseHttpHeaderForClientPrinciple(HttpRequest req)
    {
        if (!req.Headers.TryGetValue("x-ms-client-principal", out var header))
        {
            return new ClientPrincipal();
        }

        var data = header[0];
        var decoded = Convert.FromBase64String(data);
        var json = Encoding.UTF8.GetString(decoded);
        var principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return principal ?? new ClientPrincipal();
    }
}