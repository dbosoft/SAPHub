using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SAPHub.UI.Shared;

namespace SAPHub.UI.Client;

public class ClientConfigurationProvider
{
    private readonly HttpClient _http;

    public ClientConfigurationProvider(HttpClient http)
    {
        _http = http;
    }
    public Task<ClientConfiguration> GetConfiguration()
    {
        return _http.GetFromJsonAsync<ClientConfiguration>("configuration");
    }
}