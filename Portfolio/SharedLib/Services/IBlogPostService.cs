namespace SharedLib.Services;

public interface IBlogPostService
{
    Task<int> AddBlogPostAsync(BlogPost blogPost);
    Task UpdateBlogPostAsync(BlogPost blogPost);
    Task DeleteBlogPostAsync(BlogPost blogPost);
    Task<List<BlogPost>> GetBlogPostsAsync();
}