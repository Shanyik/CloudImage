namespace CloudImage.Service;

public interface IApiKeyService
{ 
    bool IsValidApiKey(string apiKey);
}