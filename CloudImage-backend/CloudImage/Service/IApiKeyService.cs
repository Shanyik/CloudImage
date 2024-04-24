namespace CloudImage.Service;

public interface IApiKeyService
{ 
    bool IsValidApiKey(string apiKey);
    void AddApiKey(string apiKey);
    double GetRemainingStorage(string apiKey);
    void UpdateUsedStorage(string apiKey, double uploadedFileSize);
    int GetRemainingSlots();
}