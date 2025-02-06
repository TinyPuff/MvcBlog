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

[Authorize(Roles = "Admin")]
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
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;

        var dashboardInfo = new AdminIndexVM
        {
            TotalPosts = _context.Post.Count(),
            TotalUsers = _userManager.Users.Count(),
            TotalComments = _context.Comment.Count(),
            TotalCategories = _context.Categories.Count()
        };
        return View(dashboardInfo);
    }

    // GET
    public async Task<IActionResult> Posts(int? pageNumber, int? pageSize, string? sortOrder, string? currentFilter, string? searchString)
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

    public async Task<IActionResult> Categories(int? pageNumber, int? pageSize, string? sortOrder, string? currentFilter, string? searchString)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
        ViewData["IDSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
        ViewData["CurrentFilter"] = searchString;

        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        var categories = _context.Categories
            .AsQueryable();

        if (!String.IsNullOrEmpty(searchString))
        {
            categories = categories.Where(p => p.Title.ToLower().Contains(searchString.ToLower()));
        }

        switch (sortOrder)
        {
            case "Title":
                categories = categories.OrderBy(p => p.Title);
                break;
            case "title_desc":
                categories = categories.OrderByDescending(p => p.Title);
                break;
            case "id_desc":
                categories = categories.OrderByDescending(p => p.ID);
                break;
            default:
                categories = categories.OrderBy(p => p.ID);
                break;
        }

        var paginatedCategories = await PaginatedList<Category>.CreateAsync(categories, pageNumber ?? 1, pageSize ?? 5);

        var adminCategoriesVM = new AdminCategoriesVM
        {
            Categories = paginatedCategories
        };

        ViewBag.PageSize = pageSize ?? 5;
        ViewBag.TotalPages = categories.Count();
        ViewBag.StartItem = (((pageNumber ?? 1) - 1) * (pageSize ?? 5)) + 1; // Shows the index of the first item on the table
        ViewBag.EndItem = Math.Min(ViewBag.StartItem + (pageSize ?? 5) - 1, categories.Count()); // Shows the index of the last item on the table

        return View(adminCategoriesVM);
    }

    public async Task<IActionResult> Comments(int? pageNumber, int? pageSize, string? sortOrder, string? currentFilter, string? searchString)
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

        var comments = _context.Comment
            .Include(p => p.Post)
            .Include(p => p.Author)
            .AsQueryable();

        if (!String.IsNullOrEmpty(searchString))
        {
            comments = comments.Where(p => p.Post.Title.ToLower().Contains(searchString.ToLower())
                || p.Author.UserName.ToLower().Contains(searchString.ToLower()));
        }

        switch (sortOrder)
        {
            case "Title":
                comments = comments.OrderBy(p => p.Post.Title);
                break;
            case "title_desc":
                comments = comments.OrderByDescending(p => p.Post.Title);
                break;
            case "id_desc":
                comments = comments.OrderByDescending(p => p.ID);
                break;
            case "Create":
                comments = comments.OrderBy(p => p.CreatedAt);
                break;
            case "create_desc":
                comments = comments.OrderByDescending(p => p.CreatedAt);
                break;
            case "Update":
                comments = comments.OrderBy(p => p.UpdatedAt);
                break;
            case "update_desc":
                comments = comments.OrderByDescending(p => p.UpdatedAt);
                break;
            default:
                comments = comments.OrderBy(p => p.ID);
                break;
        }

        var paginatedComments = await PaginatedList<Comment>.CreateAsync(comments, pageNumber ?? 1, pageSize ?? 5);

        var adminCommentsVM = new AdminCommentsVM
        {
            Comments = paginatedComments
        };

        ViewBag.PageSize = pageSize ?? 5;
        ViewBag.TotalPages = comments.Count();
        ViewBag.StartItem = (((pageNumber ?? 1) - 1) * (pageSize ?? 5)) + 1; // Shows the index of the first item on the table
        ViewBag.EndItem = Math.Min(ViewBag.StartItem + (pageSize ?? 5) - 1, comments.Count()); // Shows the index of the last item on the table

        return View(adminCommentsVM);
    }

    public async Task<IActionResult> Users(int? pageNumber, int? pageSize, string? sortOrder, string? currentFilter, string? searchString)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
        ViewData["UsernameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "username_desc" : "";
        ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
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

        var users = _userManager.Users.AsQueryable();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            user.Roles = string.Join(", ", roles);
        }

        if (!String.IsNullOrEmpty(searchString))
        {
            users = users.Where(p => p.UserName.ToLower().Contains(searchString.ToLower())
                || p.Name.ToLower().Contains(searchString.ToLower())
                || p.Roles.ToLower().Contains(searchString.ToLower())
                || p.Email.ToLower().Contains(searchString.ToLower()));
        }

        switch (sortOrder)
        {
            case "Email":
                users = users.OrderBy(p => p.Email);
                break;
            case "email_desc":
                users = users.OrderByDescending(p => p.Email);
                break;
            case "username_desc":
                users = users.OrderByDescending(p => p.UserName);
                break;
            case "Name":
                users = users.OrderBy(p => p.Name);
                break;
            case "name_desc":
                users = users.OrderByDescending(p => p.Name);
                break;
            default:
                users = users.OrderBy(p => p.UserName);
                break;
        }

        var paginatedUsers = await PaginatedList<BlogUser>.CreateAsync(users, pageNumber ?? 1, pageSize ?? 5);

        var adminUsersVM = new AdminUsersVM
        {
            Users = paginatedUsers
        };

        ViewBag.PageSize = pageSize ?? 5;
        ViewBag.TotalPages = users.Count();
        ViewBag.StartItem = (((pageNumber ?? 1) - 1) * (pageSize ?? 5)) + 1; // Shows the index of the first item on the table
        ViewBag.EndItem = Math.Min(ViewBag.StartItem + (pageSize ?? 5) - 1, users.Count()); // Shows the index of the last item on the table

        return View(adminUsersVM);
    }
}