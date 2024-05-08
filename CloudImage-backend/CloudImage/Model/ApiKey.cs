using System.ComponentModel.DataAnnotations.Schema;

namespace CloudImage.Model;

public class ApiKey
{
    public int Id { get; set; }
    public AppUser? User { get; set; }
    public string Key { get; set; }
    public double AllocatedStorageGB { get; set; }
    public double UsedStorageGB { get; set; }
}