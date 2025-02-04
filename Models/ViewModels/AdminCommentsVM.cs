using MvcBlog.Data;

namespace MvcBlog.Models.ViewModels;

public class AdminCommentsVM
{
    public PaginatedList<Comment> Comments { get; set; }
}