namespace SharedLib.Services;

public interface IBlogPostService
{
    Task<int> AddBlogPost(BlogPost blogPost);
    Task UpdateBlogPost(BlogPost blogPost);
    Task DeleteBlogPost(BlogPost blogPost);
    Task<List<BlogPost>> GetBlogPosts();
}