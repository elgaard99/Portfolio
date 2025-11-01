using System.ComponentModel.DataAnnotations;

namespace SharedLib;

public class Photo
{
    public Guid Id { get; init; }
    public string? Description { get; set; } = string.Empty;
    public string? BucketName { get; set; }  = string.Empty;
    public string? ObjectName { get; set; } = string.Empty;
    
    // Foreign key and navigation property
    public Guid BlogPostId { get; init; }
    public BlogPost BlogPost { get; init; }
    
}