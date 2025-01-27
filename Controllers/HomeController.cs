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

    public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
    {
        var posts = _context.Post
                    .Include(p => p.Author)
                    .OrderByDescending(p => p.CreatedAt);

        int pageSize = 3;
        var paginatedPosts = await PaginatedList<Post>.CreateAsync(posts, pageNumber ?? 1, pageSize);

        var postsVM = new PostVM
        {
            Posts = paginatedPosts
        };

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
