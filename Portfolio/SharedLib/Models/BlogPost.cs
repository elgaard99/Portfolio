namespace SharedLib;

public class BlogPost
{
    public Guid Id { get; init; }
    public string Title { get; set; }
    public string Goal { get; set; }
    public string Content { get; set; }

    private DateTime _publishDate;
    public DateTime PublishDate
    {
        get => _publishDate;
        set => _publishDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    
    public bool IsVisible { get; set; }

    public List<Photo> Photos { get; set; } = new();
}