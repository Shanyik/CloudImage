namespace CloudImage.Model;

public class ApiKey
{
    private static int _nextId = 1;
    public int Id { get; set; }
    public string Key { get; set; }
    public double AllocatedStorageGB { get; set; }
    public double UsedStorageGB { get; set; }
    public ApiKey()
    {
        // Assign a unique ID when a new ApiKey object is created
        Id = _nextId++;
    }
}