using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcBlog.Data;

namespace MvcBlog.Models;

public class Post
{
    [Required]
    public int ID { get; set; }

    [Required]
    [StringLength(150, ErrorMessage = "Title must be between 3 and 150 characters long.", MinimumLength = 3)]
    public string Title { get; set;} = string.Empty;

    [Required]
    [StringLength(400, ErrorMessage = "Text must be between 50 and 400 characters long.", MinimumLength = 50)]
    public string Body { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
    public DateTime CreatedAt { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
    public DateTime UpdatedAt { get; set; }

    public string AuthorID { get; set; }
    
    [Required]
    public BlogUser Author { get; set; }

    public ICollection<Comment>? Comments { get; set; }

    // public ICollection<Category>? Categories { get; set; }
}