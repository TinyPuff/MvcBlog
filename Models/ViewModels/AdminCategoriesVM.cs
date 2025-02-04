using MvcBlog.Data;

namespace MvcBlog.Models.ViewModels;

public class AdminCategoriesVM
{
    public PaginatedList<Category> Categories { get; set; }
}