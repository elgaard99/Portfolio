using Microsoft.EntityFrameworkCore;
using SharedLib.Data;

namespace SharedLib.Services;

public class BlogPostService : IBlogPostService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public BlogPostService(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    }

    public async Task<Guid> AddBlogPostAsync(BlogPost blogPost)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.BlogPosts.Add(blogPost);
        await dbContext.SaveChangesAsync();
        return blogPost.Id;
    }

    public async Task UpdateBlogPostAsync(BlogPost blogPost)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var existing = await dbContext.BlogPosts.FindAsync(blogPost.Id);
        if (existing is null)
            throw new InvalidOperationException($"BlogPost with ID {blogPost.Id} not found.");

        existing.Title = blogPost.Title;
        existing.Goal = blogPost.Goal;
        existing.Content = blogPost.Content;
        existing.PublishDate = blogPost.PublishDate;
        existing.IsVisible = blogPost.IsVisible;
        existing.Photos = blogPost.Photos;

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteBlogPostAsync(BlogPost blogPost)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var existing = await dbContext.BlogPosts.FindAsync(blogPost.Id);
        if (existing is null)
            throw new InvalidOperationException($"BlogPost with ID {blogPost.Id} not found.");

        dbContext.BlogPosts.Remove(existing);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<BlogPost>> GetBlogPostsAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.BlogPosts
            .Include(bp => bp.Photos)
            .OrderByDescending(b => b.PublishDate)
            .ToListAsync();
    }
}