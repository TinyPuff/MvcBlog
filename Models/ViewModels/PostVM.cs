using MvcBlog.Data;

namespace MvcBlog.Models.ViewModels;

public class PostVM
{
    public PaginatedList<Post> Posts { get; set; }
}