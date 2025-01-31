using MvcBlog.Data;
using System.ComponentModel.DataAnnotations;

namespace MvcBlog.Models.ViewModels;

public class PostDetailsVM
{
    // properties for comments
    [Required]
    public int ID { get; set; }

    [Required]
    [StringLength(150, ErrorMessage = "Comment cannot have more than 150 characters.")]
    public string Body { get; set; } = string.Empty;

    public Post Post { get; set; }

    public PaginatedList<Comment>? Comments { get; set; }

    public ICollection<Category>? Categories { get; set; }
}