using CloudImage.Model;
using Newtonsoft.Json;

namespace CloudImage.Service;

public class ApiKeyService : IApiKeyService
{
    private List<ApiKey> _apiKeys;

    public ApiKeyService()
    {
        LoadApiKeys();
    }

    private void LoadApiKeys()
    {
        // Load API keys from a JSON file
        var json = File.ReadAllText("ApiKeys.json");
        _apiKeys = JsonConvert.DeserializeObject<List<ApiKey>>(json);
    }

    public bool IsValidApiKey(string apiKey)
    {
        return _apiKeys.Any(k => k.Key == apiKey);
    }
}