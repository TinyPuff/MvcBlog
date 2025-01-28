using MvcBlog.Data;

namespace MvcBlog.Models.ViewModels;

public class HomeVM
{
    public PaginatedList<Post> Posts { get; set; }
}