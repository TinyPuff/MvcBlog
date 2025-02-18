using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcBlog.Data;
using MvcBlog.Models;
using Microsoft.AspNetCore.Identity;
using Humanizer;
using MvcBlog.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MvcBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public PostsController(BlogDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Posts
        [Authorize]
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
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
                        .Where(p => p.AuthorID == _userManager.GetUserId(User))
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

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id, int? pageNumber)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .Include(c => c.Categories)
                .Include(c => c.Comments!)
                    .ThenInclude(i => i.Author)
                .FirstOrDefaultAsync(p => p.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            int pageSize = 3;

            IQueryable<Comment> commentsIQ = from c in _context.Comment
                                             select c;

            commentsIQ = commentsIQ.Where(c => c.PostID == post.ID).OrderByDescending(d => d.CreatedAt);

            var comments = await PaginatedList<Comment>.CreateAsync(commentsIQ, pageNumber ?? 1, pageSize);

            var postDetails = new PostDetailsVM()
            {
                Body = "",
                Post = post,
                Comments = comments
            };

            return View(postDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, [Bind("ID, Body")] PostDetailsVM comment)
        {
            var currentUser = await _userManager.GetUserAsync(User); // gives us the current logged-in user

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .Include(c => c.Comments!)
                    .ThenInclude(i => i.Author)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (post == null)
            {
                return NotFound();
            }

            var newComment = new Comment()
            {
                Body = comment.Body,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorID = currentUser.Id,
                Author = currentUser,
                PostID = post.ID,
                Post = post
            };

            _context.Add(newComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details));
        }

        // GET: Posts/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Body,SelectedCategories")] PostVM post)
        {
            var currentUser = await _userManager.GetUserAsync(User); // gives us the current logged-in user

            var categories = _context.Categories
                .Where(c => post.SelectedCategories.Contains(c.ID))
                .ToList();

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var newPost = new Post()
                {
                    Title = post.Title,
                    Body = post.Body,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    AuthorID = currentUser.Id,
                    Author = currentUser,
                    Categories = categories
                };

                _context.Add(newPost);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = newPost.ID });
            }


            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.ID == id);

            if (post == null)
            {
                return NotFound();
            }

            if (currentUser != post.Author)
            {
                if (await _userManager.IsInRoleAsync(currentUser, "Admin") == false)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            var categories = _context.Categories.ToList();
            var selectedCategories = post.Categories.ToList();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategories = selectedCategories;

            var postVM = new PostVM
            {
                ID = post.ID,
                Title = post.Title,
                Body = post.Body
            };

            return View(postVM);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Body,SelectedCategories")] PostVM post)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (id != post.ID)
            {
                return NotFound();
            }

            var postToUpdate = await _context.Post
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (currentUser != postToUpdate.Author)
            {
                if (await _userManager.IsInRoleAsync(currentUser, "Admin") == false)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            if (postToUpdate == null)
            {
                return NotFound();
            }

            var categories = _context.Categories
                .Where(c => post.SelectedCategories.Contains(c.ID))
                .ToList();

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                postToUpdate.Title = post.Title;
                postToUpdate.Body = post.Body;
                postToUpdate.UpdatedAt = DateTime.Now;
                postToUpdate.Categories = categories;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = postToUpdate.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(postToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (currentUser != post.Author)
            {
                if (await _userManager.IsInRoleAsync(currentUser, "Admin") == false)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var post = await _context.Post.FindAsync(id);

            if (post != null)
            {
                _context.Post.Remove(post);
            }

            if (currentUser != post.Author)
            {
                if (await _userManager.IsInRoleAsync(currentUser, "Admin") == false)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.ID == id);
        }
    }
}
