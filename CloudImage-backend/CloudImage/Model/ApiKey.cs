using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace CloudImage.Model;

public class ApiKey
{
    public int Id { get; set; }
    public string Key { get; set; }
    public double AllocatedStorageGB { get; set; }
    public double UsedStorageGB { get; set; }
    public AppUser? User { get; set; }
    public List<Image> Images { get; set; }
}