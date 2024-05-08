using CloudImage.Model;

namespace CloudImage.Repository;

public interface IImageRepository
{
    Task<IEnumerable<Image>> GetAll();
    Task<Image?> GetByUrl(string url);
    Task Add(Image image);
    Task Delete(string url);
}