using MvcBlog.Models;
using MvcBlog.Models.ViewModels;
using MvcBlog.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace MvcBlog.Controllers;

public class AdminController : Controller
{
    private readonly BlogDbContext _context;
    private UserManager<BlogUser> _userManager;

    public AdminController(BlogDbContext context, UserManager<BlogUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    // GET
    public async Task<IActionResult> Posts(int? pageNumber, int? pageSize)
    {
        var posts = _context.Post.Include(p => p.Author).OrderByDescending(d => d.CreatedAt);
        var paginatedPosts = await PaginatedList<Post>.CreateAsync(posts, pageNumber ?? 1, pageSize ?? 5);

        var adminPostsVM = new AdminPostsVM
        {
            Posts = paginatedPosts
        };

        ViewBag.PageSize = pageSize ?? 5;
        ViewBag.TotalPages = posts.Count();
        ViewBag.StartItem = (((pageNumber ?? 1) - 1) * (pageSize ?? 5)) + 1; // Shows the index of the first item on the table
        ViewBag.EndItem = Math.Min(ViewBag.StartItem + (pageSize ?? 5) - 1, posts.Count()); // Shows the index of the last item on the table

        return View(adminPostsVM);
    }

}