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

    public async Task<int> AddBlogPost(BlogPost blogPost)
    {
        _context.BlogPosts.Add(blogPost);
        await _context.SaveChangesAsync();
        return blogPost.Id;
    }

    public async Task UpdateBlogPost(BlogPost blogPost)
    {
        // Optional: check if entity exists
        var existing = await _context.BlogPosts.FindAsync(blogPost.Id);
        if (existing is null)
            throw new InvalidOperationException($"BlogPost with ID {blogPost.Id} not found.");

        existing.Title = blogPost.Title;
        existing.Goal = blogPost.Goal;
        existing.Content = blogPost.Content;
        existing.PublishDate = blogPost.PublishDate;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteBlogPost(BlogPost blogPost)
    {
        var existing = await _context.BlogPosts.FindAsync(blogPost.Id);
        if (existing is null)
            throw new InvalidOperationException($"BlogPost with ID {blogPost.Id} not found.");

        _context.BlogPosts.Remove(existing);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BlogPost>> GetBlogPosts()
    {
        return await _context.BlogPosts
            .OrderByDescending(b => b.PublishDate)
            .ToListAsync();
    }
}