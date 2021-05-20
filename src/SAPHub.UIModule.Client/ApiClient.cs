using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SAPHub.UI.Shared;

namespace SAPHub.UI.Client
{
    public class ApiClient
    {
        private readonly ClientConfigurationProvider _configurationProvider;
        private HttpClient _cachedHttpClient;

        public ApiClient(ClientConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        private async Task<HttpClient> GetHttpClient()
        {
            if (_cachedHttpClient != null)
                return _cachedHttpClient;

            var config = await _configurationProvider.GetConfiguration();

            _cachedHttpClient = new HttpClient {BaseAddress = new Uri(config.ApiEndpoint)};
            return _cachedHttpClient;

        }

        public async Task<Operation> GetCompanyCodes()
        {
            return await (await GetHttpClient()).GetFromJsonAsync<Operation>("companycodes");
        }

        public async Task<OperationResult<Company>> GetCompanyCodesResult(Guid id)
        {
            return await (await GetHttpClient()).GetFromJsonAsync<OperationResult<Company>>($"companycodes/result/{id}");
        }

        public async Task<Operation> GetOperation(Guid id)
        {
            return await (await GetHttpClient()).GetFromJsonAsync<Operation>($"operations/{id}");
        }
    }
}