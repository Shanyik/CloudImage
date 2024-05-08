using CloudImage.Model;

namespace CloudImage.Repository;

public interface IApiRepository
{
    Task<IEnumerable<ApiKey>> GetAll();
    Task<ApiKey?> GetByKey(string key);
    Task Update(ApiKey apiKey);
}