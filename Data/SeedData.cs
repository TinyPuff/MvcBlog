using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcBlog.Data;
using MvcBlog.Models;
using NuGet.Configuration;

namespace MvcBlog.Data;

public class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, BlogDbContext context)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<BlogUser>>();

            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var users = new[]
            {
                new { Username = "admin", Name = "admin", Email = "admin@admin.com", Password = "Admin@123", Role = "Admin" },
                new { Username = "testuser", Name = "testuser", Email = "testuser@email.com", Password = "Testuser@123", Role = "User" }
            };

            foreach (var userInfo in users)
            {
                if (userManager.Users.Count() == users.Count())
                {
                    break;
                }

                if (userManager.Users.Any(u => u.NormalizedUserName == userInfo.Username.ToUpper()))
                {
                    continue;
                }

                var user = new BlogUser
                {
                    UserName = userInfo.Username,
                    Name = userInfo.Name,
                    Email = userInfo.Email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, userInfo.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userInfo.Role);
                }
            }

            if (context.Post.Any())
            {
                return;
            }

            var admin = await userManager.FindByNameAsync("admin");
            var testUser = await userManager.FindByNameAsync("testuser");

            var categories = new Category[]
            {
                new Category
                {
                    Title = "Boring"
                },
                new Category
                {
                    Title = "More Boring"
                },
                new Category
                {
                    Title = "Less Boring"
                },
                new Category
                {
                    Title = "Not Boring"
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            var posts = new Post[]
            {
                new Post
                {
                    Title = "Post #1",
                    Body = "<p>Text<br><br><em>Italic text</em><br><br><strong>Bold Text</strong><br><u><br></u><u>Underlined text</u></p>",
                    CreatedAt = DateTime.Parse("2025-02-06 11:01"),
                    UpdatedAt = DateTime.Parse("2025-02-06 11:01"),
                    Categories = categories[0..2],
                    Author = admin,
                    AuthorID = admin.Id
                },

                new Post
                {
                    Title = "Post #2",
                    Body = "<p>Text<br><br><em>Italic text</em><br><br><strong>Bold Text</strong><br><u><br></u><u>Underlined text</u></p>",
                    CreatedAt = DateTime.Parse("2025-02-07 11:05"),
                    UpdatedAt = DateTime.Parse("2025-02-07 11:05"),
                    Categories = categories[1..2],
                    Author = testUser,
                    AuthorID = testUser.Id
                },

                new Post
                {
                    Title = "Post #3",
                    Body = "<p>Text<br><br><em>Italic text</em><br><br><strong>Bold Text</strong><br><u><br></u><u>Underlined text</u></p>",
                    CreatedAt = DateTime.Parse("2025-02-08 11:10"),
                    UpdatedAt = DateTime.Parse("2025-02-08 11:10"),
                    Categories = categories[2..2],
                    Author = testUser,
                    AuthorID = testUser.Id
                },

                new Post
                {
                    Title = "Post #4",
                    Body = "<p>Text<br><br><em>Italic text</em><br><br><strong>Bold Text</strong><br><u><br></u><u>Underlined text</u></p>",
                    CreatedAt = DateTime.Parse("2025-02-09 11:15"),
                    UpdatedAt = DateTime.Parse("2025-02-09 11:15"),
                    Categories = categories[0..2],
                    Author = admin,
                    AuthorID = admin.Id
                }
            };

            await context.AddRangeAsync(posts);
            await context.SaveChangesAsync();

            var comments = new Comment[]
            {
                new Comment
                {
                    Body = "Test comment #1",
                    CreatedAt = DateTime.Parse("2025-02-10 11:01"),
                    UpdatedAt = DateTime.Parse("2025-02-10 11:01"),
                    Author = admin,
                    AuthorID = admin.Id,
                    Post = posts[0],
                    PostID = posts[0].ID
                },

                new Comment
                {
                    Body = "Test comment #2",
                    CreatedAt = DateTime.Parse("2025-02-10 11:02"),
                    UpdatedAt = DateTime.Parse("2025-02-10 11:02"),
                    Author = admin,
                    AuthorID = admin.Id,
                    Post = posts[0],
                    PostID = posts[0].ID
                },

                new Comment
                {
                    Body = "Test comment #3",
                    CreatedAt = DateTime.Parse("2025-02-10 11:03"),
                    UpdatedAt = DateTime.Parse("2025-02-10 11:03"),
                    Author = testUser,
                    AuthorID = testUser.Id,
                    Post = posts[0],
                    PostID = posts[0].ID
                },

                new Comment
                {
                    Body = "Test comment #4",
                    CreatedAt = DateTime.Parse("2025-02-10 11:04"),
                    UpdatedAt = DateTime.Parse("2025-02-10 11:04"),
                    Author = testUser,
                    AuthorID = testUser.Id,
                    Post = posts[0],
                    PostID = posts[0].ID
                }
            };

            await context.Comment.AddRangeAsync(comments);
            await context.SaveChangesAsync();

            foreach (var post in posts)
            {
                if (post.ID != 1)
                {
                    post.Comments = comments;
                }
            }
        }
    }
}
