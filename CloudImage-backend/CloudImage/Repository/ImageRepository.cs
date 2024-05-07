using CloudImage.Data;
using CloudImage.Model;
using Microsoft.EntityFrameworkCore;

namespace CloudImage.Repository;

public class ImageRepository : IImageRepository
{
    private readonly AppDbContext _context;
    
    public ImageRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<ApiKey>> GetAll()
    {
        return await _context.ApiKeys.Include(c => c.User).AsNoTracking().ToListAsync();
    }
    
    public async Task<ApiKey?> GetByKey(string key)
    {
        return await _context.ApiKeys.FirstOrDefaultAsync(c => c.Key == key);
    }
    
    public async Task Update(ApiKey apiKey)
    {
        _context.Entry(apiKey).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}