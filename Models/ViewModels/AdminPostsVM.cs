using MvcBlog.Data;

namespace MvcBlog.Models.ViewModels;

public class AdminPostsVM
{
    public PaginatedList<Post> Posts { get; set; }
}