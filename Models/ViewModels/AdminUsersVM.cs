using MvcBlog.Data;

namespace MvcBlog.Models.ViewModels;

public class AdminUsersVM
{
    public PaginatedList<BlogUser> Users { get; set; }
}