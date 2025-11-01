using Microsoft.EntityFrameworkCore;
using SharedLib.Data;

namespace SharedLib.Services;

public class BlogPostService : IBlogPostService
{
    private readonly AppDbContext _context;

    public BlogPostService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddBlogPostAsync(BlogPost blogPost)
    {
        _context.BlogPosts.Add(blogPost);
        await _context.SaveChangesAsync();
        return blogPost.Id;
    }

    public async Task UpdateBlogPostAsync(BlogPost blogPost)
    {
        // Optional: check if entity exists
        var existing = await _context.BlogPosts.FindAsync(blogPost.Id);
        if (existing is null)
            throw new InvalidOperationException($"BlogPost with ID {blogPost.Id} not found.");

        existing.Title = blogPost.Title;
        existing.Goal = blogPost.Goal;
        existing.Content = blogPost.Content;
        existing.PublishDate = blogPost.PublishDate;
        existing.IsVisible = blogPost.IsVisible;
        existing.Photos = blogPost.Photos;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteBlogPostAsync(BlogPost blogPost)
    {
        var existing = await _context.BlogPosts.FindAsync(blogPost.Id);
        if (existing is null)
            throw new InvalidOperationException($"BlogPost with ID {blogPost.Id} not found.");

        _context.BlogPosts.Remove(existing);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BlogPost>> GetBlogPostsAsync()
    {
        return await _context.BlogPosts
            .Include(bp => bp.Photos)
            .OrderByDescending(b => b.PublishDate)
            .ToListAsync();
    }
}