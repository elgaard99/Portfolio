namespace SharedLib;

public class Photo
{
    public Guid Id { get; private set; }
    public string Description { get; set; }
    public string BucketName { get; }
    public string ObjectName { get; } // e.g. "photos/2025/10/myimage123.jpg"
    
    // Foreign key and navigation property
    public int BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; } = null!;
}