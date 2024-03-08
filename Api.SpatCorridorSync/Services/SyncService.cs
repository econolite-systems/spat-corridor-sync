using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.SpatCorridorSync;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Econolite.Ode.Authorization;

namespace Econolite.Ode.Api.SpatCorridorSync.Services;

public class SyncService
{
    private readonly string _basePath;
    private readonly HttpClient _httpClient;
    private readonly ITokenHandler _tokenHandler;

    public SyncService(IConfiguration configuration, HttpClient httpClient, ITokenHandler tokenHandler)
    {
        _basePath = configuration.GetValue<string>("Sync:BasePath") ?? throw new NullReferenceException("Configuration Sync:BasePath setting");
        _httpClient = httpClient;
        _tokenHandler = tokenHandler;
    }

    public async Task<bool> SendToEntitySync(EntitySync data, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = JsonSerializer.Serialize(data, JsonPayloadSerializerOptions.Options);
            var accessToken = await _tokenHandler.GetTokenAsync(cancellationToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var responseMessage = await _httpClient.PostAsync($"{_basePath}/entities-sync", new StringContent(payload, Encoding.Default, mediaType: "application/json"), cancellationToken);
            return responseMessage.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}