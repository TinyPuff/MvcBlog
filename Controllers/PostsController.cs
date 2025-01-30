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
        public async Task<IActionResult> Index()
        {
            var blogDbContext = _context.Post.Include(p => p.Author);
            return View(await blogDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .Include(p => p.Comments.OrderByDescending(c => c.CreatedAt))
                .FirstOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            var postDetails = new PostDetailsVM()
            {
                Body = "",
                Post = post
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
                .Include(c => c.Comments)
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
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Body")] PostVM post)
        {
            var currentUser = await _userManager.GetUserAsync(User); // gives us the current logged-in user

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
                    Author = currentUser
                };

                _context.Add(newPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            

            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Body")] PostVM post)
        {   
            if (id != post.ID)
            {
                return NotFound();
            }

            var postToUpdate = await _context.Post
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (postToUpdate == null)
            {
                return NotFound();
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                postToUpdate.Title = post.Title;
                postToUpdate.Body = post.Body;
                postToUpdate.UpdatedAt = DateTime.Now;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            if (post != null)
            {
                _context.Post.Remove(post);
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
