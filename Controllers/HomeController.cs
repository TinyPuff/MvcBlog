using System.Data.Common;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcBlog.Models;
using MvcBlog.Models.ViewModels;
using System.Linq;
using MvcBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace MvcBlog.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BlogDbContext _context;

    public HomeController(ILogger<HomeController> logger, BlogDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        /* var allPosts = (from post in db.
                       select new PostVM
                       {
                            ID = post.ID,
                            Title = post.Title,
                            Body = post.Body,
                            CreatedAt = post.CreatedAt,
                            UpdatedAt = post.UpdatedAt,
                            Author = post.Author
                       })
                       .OrderByDescending(p => p.ID)
                       .ToList(); */
        var posts = _context.Post
                    .Include(p => p.Author)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();

        var postsVM = new PostVM
        {
            Posts = posts
        };

        Console.WriteLine($"Posts count: {posts.Count}");
        Console.WriteLine($"VM Posts count: {postsVM.Posts.Count}");
        return View(postsVM);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
