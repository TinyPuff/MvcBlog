using MvcBlog.Data;

namespace MvcBlog.Models.ViewModels;

public class AdminIndexVM
{
    public PostVM Post { get; set; }

    public int TotalPosts { get; set; }

    public int TotalUsers { get; set; }

    public int TotalComments { get; set; }

    public int TotalCategories { get; set; }
}