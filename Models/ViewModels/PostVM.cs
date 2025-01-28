using MvcBlog.Data;
using System.ComponentModel.DataAnnotations;

namespace MvcBlog.Models.ViewModels;

public class PostVM
{
    [Required]
    public int ID { get; set; }

    [Required]
    [StringLength(150, ErrorMessage = "Title must be between 3 and 150 characters long.", MinimumLength = 3)]
    public string Title { get; set;} = string.Empty;

    [Required]
    [StringLength(400, ErrorMessage = "Text must be between 50 and 400 characters long.", MinimumLength = 50)]
    public string Body { get; set; } = string.Empty;

    public ICollection<Comment>? Comments { get; set; }
}