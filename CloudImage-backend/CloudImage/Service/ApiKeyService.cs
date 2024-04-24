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
        string json = null;
        try
        {
            json = File.ReadAllText("ApiKeys.json");
        }
        catch (FileNotFoundException)
        {
            // Handle file not found exception (e.g., create a new empty file)
            File.WriteAllText("ApiKeys.json", "[]");
        }
    
        _apiKeys = JsonConvert.DeserializeObject<List<ApiKey>>(json) ?? new List<ApiKey>();
    }

    public bool IsValidApiKey(string apiKey)
    {
        return _apiKeys.Any(k => k.Key == apiKey);
    }
    
    public void AddApiKey(string apiKey)
    {
        _apiKeys.Add(new ApiKey { Key = apiKey, AllocatedStorageGB = 1.0, UsedStorageGB = 0});
        SaveApiKeysToFile();
    }
    
    public double GetRemainingStorage(string apiKey)
    {
        var apiKeyInfo = _apiKeys.FirstOrDefault(k => k.Key == apiKey);
        if (apiKeyInfo != null)
        {
            return apiKeyInfo.AllocatedStorageGB - apiKeyInfo.UsedStorageGB;
        }
        return 0;
    }
    
    public void UpdateUsedStorage(string apiKey, long uploadedFileSize)
    {
        var apiKeyInfo = _apiKeys.FirstOrDefault(k => k.Key == apiKey);
        if (apiKeyInfo != null)
        {
            apiKeyInfo.UsedStorageGB += (double)uploadedFileSize / (1024 * 1024 * 1024); // Convert bytes to GB
            SaveApiKeysToFile();
        }
    }

    private void SaveApiKeysToFile()
    {
        var json = JsonConvert.SerializeObject(_apiKeys);
        File.WriteAllText("ApiKeys.json", json);
    }
}