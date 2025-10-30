namespace SharedLib;

public class BlogPost
{
    public int Id { get; init; }
    public string Title { get; set; }
    public string Goal { get; set; }
    public string Content { get; set; }
    public DateTime PublishDate { get; set; }

    // public BlogPost(int id, string title, string goal, string content, DateTime publishDate)
    // {
    //     Id = id;
    //     Title = title;
    //     Goal = goal;
    //     Content = content;
    //     PublishDate = publishDate;
    // }
}