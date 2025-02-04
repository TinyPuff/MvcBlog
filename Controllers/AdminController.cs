using MvcBlog.Models;
using MvcBlog.Models.ViewModels;
using MvcBlog.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    // GET
    [Authorize]
    public async Task<IActionResult> Posts(int? pageNumber, int? pageSize, string? sortOrder, string? currentFilter,  string? searchString)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
        ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
        ViewData["CreateSortParm"] = sortOrder == "Create" ? "create_desc" : "Create";
        ViewData["UpdateSortParm"] = sortOrder == "Update" ? "update_desc" : "Update";
        ViewData["CurrentFilter"] = searchString;

        var currentUser = await _userManager.GetUserAsync(User);
        ViewData["Username"] = currentUser.UserName;

        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        var posts = _context.Post
            .Include(p => p.Author)
            .AsQueryable();

        if (!String.IsNullOrEmpty(searchString))
        {
            posts = posts.Where(p => p.Title.ToLower().Contains(searchString.ToLower()) 
                || p.Author.UserName.ToLower().Contains(searchString.ToLower()));
        }

        switch (sortOrder)
        {
            case "Title":
                posts = posts.OrderBy(p => p.Title);
                break;
            case "title_desc":
                posts = posts.OrderByDescending(p => p.Title);
                break;
            case "id_desc":
                posts = posts.OrderByDescending(p => p.ID);
                break;
            case "Create":
                posts = posts.OrderBy(p => p.CreatedAt);
                break;
            case "create_desc":
                posts = posts.OrderByDescending(p => p.CreatedAt);
                break;
            case "Update":
                posts = posts.OrderBy(p => p.UpdatedAt);
                break;
            case "update_desc":
                posts = posts.OrderByDescending(p => p.UpdatedAt);
                break;
            default:
                posts = posts.OrderBy(p => p.ID);
                break;
        }

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