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

    public async Task<IActionResult> Index(string? searchString, string? currentFilter, int? pageNumber)
    {
        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;

        var posts = _context.Post
                    .Include(p => p.Author)
                    .OrderByDescending(p => p.CreatedAt)
                    .AsQueryable();

        if (!String.IsNullOrEmpty(searchString))
        {
            posts = posts.Where(p => p.Title.ToLower().Contains(searchString.ToLower())
                || p.Body.ToLower().Contains(searchString.ToLower()));
        }

        int pageSize = 3;
        var paginatedPosts = await PaginatedList<Post>.CreateAsync(posts, pageNumber ?? 1, pageSize);

        var homeVM = new HomeVM
        {
            Posts = paginatedPosts
        };

        return View(homeVM);
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
