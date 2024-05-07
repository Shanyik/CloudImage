using CloudImage.Model;

namespace CloudImage.Repository;

public interface IImageRepository
{
    Task<IEnumerable<ApiKey>> GetAll();
    Task<ApiKey?> GetByKey(string key);
    Task Update(ApiKey apiKey);
}