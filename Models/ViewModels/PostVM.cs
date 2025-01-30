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
    [StringLength(400, ErrorMessage = "Text must have less than 400 characters.")]
    public string Body { get; set; } = string.Empty;
}