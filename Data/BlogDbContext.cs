using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcBlog.Data;
using MvcBlog.Models;

namespace MvcBlog.Data;

public class BlogDbContext : IdentityDbContext<BlogUser>
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

public DbSet<Post> Post { get; set; } = default!; // should change this to Posts
public DbSet<Comment> Comment { get; set; } // should change this to Comments
public DbSet<Category> Categories { get; set; }
}
