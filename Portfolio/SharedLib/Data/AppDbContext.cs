using Microsoft.EntityFrameworkCore;

namespace SharedLib.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Photo> Photos => Set<Photo>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // BlogPost–Photo relationship
        modelBuilder.Entity<Photo>()
            .HasOne(p => p.BlogPost)
            .WithMany(b => b.Photos)
            .HasForeignKey(p => p.BlogPostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional: Configure table names, etc.
        modelBuilder.Entity<BlogPost>().ToTable("BlogPosts");
        modelBuilder.Entity<Photo>().ToTable("Photos");
    }
}
