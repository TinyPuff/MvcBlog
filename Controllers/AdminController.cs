using MvcBlog.Models;
using MvcBlog.Models.ViewModels;
using MvcBlog.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.ObjectModel;

namespace MvcBlog.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly BlogDbContext _context;
    private UserManager<BlogUser> _userManager;
    private RoleManager<IdentityRole> _roleManager;

    public AdminController(BlogDbContext context, UserManager<BlogUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: /Admin/
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

    // GET: /Admin/Posts/
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

    // GET: /Admin/Categories/
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

    // GET: /Admin/Comments/
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

    // GET: /Admin/Users/
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

    // GET: /Admin/Users/Edit/{user.Id}
    [Route("Admin/Users/Edit/{id}")]
    public async Task<IActionResult> UsersEdit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return RedirectToAction(nameof(Users));
        }

        var roles = _roleManager.Roles; // Fetch all roles
        var userRoles = _context.UserRoles
            .Where(ur => ur.UserId == user.Id); // Fetch all the roles of the selected user
        var selectedRoles = new Collection<string>();

        foreach (var item in userRoles) // Adding all the already assigned roles to 'selectedRoles' in order to pass it to the view
        {
            selectedRoles.Add(
                (await roles.FirstOrDefaultAsync(r => r.Id == item.RoleId)).Name
            );
        }

        var userVM = new AdminUsersEditVM
        {
            ID = user.Id,
            Username = user.UserName,
            Email = user.Email,
            SelectedRoles = selectedRoles,
            Roles = roles
        };

        return View(userVM);
    }

    // POST: /Admin/Users/Edit/{user.Id}
    [Route("Admin/Users/Edit/{id}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UsersEdit(string id, [Bind("ID,Username,Email,SelectedRoles")] AdminUsersEditVM user)
    {
        var userToUpdate = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == id);
        var userRoles = await _userManager.GetRolesAsync(userToUpdate);

        if (userToUpdate == null)
        {
            return RedirectToAction(nameof(Users));
        }

        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
        }

        if (ModelState.IsValid)
        {
            userToUpdate.UserName = user.Username;
            userToUpdate.Email = user.Email;

            await _userManager.RemoveFromRolesAsync(userToUpdate, userRoles); // Remove all previous roles

            foreach (var item in user.SelectedRoles)
            {
                await _userManager.AddToRoleAsync(userToUpdate, item); // Set the new roles
            }

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userToUpdate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        return View(user);
    }

    // GET: /Admin/Users/Delete/{user.id}
    [Route("Admin/Users/Delete/{id}")]
    public async Task<IActionResult> UsersDelete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }
        
        var userVM = new AdminUsersEditVM
        {
            ID = user.Id,
            Username = user.UserName,
            Email = user.Email
        };
        
        return View(userVM);
    }

    // POST: /Admin/Users/Delete/{user.Id}
    [Route("Admin/Users/Delete/{id}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UsersDelete(string id, [Bind("ID,Username,Email")] AdminUsersEditVM user)
    {
        var userToDelete = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        
        if (userToDelete != null)
        {
            _context.Users.Remove(userToDelete);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Users));
    }

    private bool UserExists(string id)
    {
        return _userManager.Users.Any(u => u.Id == id);
    }
}