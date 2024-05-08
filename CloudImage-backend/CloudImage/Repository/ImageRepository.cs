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
    public async Task<IEnumerable<Image>> GetAll()
    {
        return await _context.Images.ToListAsync();
    }

    public async Task<Image?> GetByUrl(string url)
    {
        return await _context.Images.FirstOrDefaultAsync(c => c.ImageUrl == url);
    }

    public async Task Add(Image image)
    {
        await _context.AddAsync(image);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(string url)
    {
        Image? image = await _context.Images.FirstOrDefaultAsync(c => c.ImageUrl == url);
        _context.Images.Remove(image);
        await _context.SaveChangesAsync();
    }
}