using System.ComponentModel.DataAnnotations;

namespace MvcBlog.Models;

public class Category
{
    public int ID { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public ICollection<Post>? Posts { get; set; }
}