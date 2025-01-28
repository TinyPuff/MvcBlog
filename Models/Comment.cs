using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MvcBlog.Data;

namespace MvcBlog.Models;

public class Comment
{
    [Required]
    public int ID { get; set; }

    [Required]
    [StringLength(150, ErrorMessage = "Comment cannot have more than 150 characters.")]
    public string Body { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime CreatedAt { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime UpdatedAt { get; set; }
    
    public string AuthorID { get; set; }

    [Required]
    public BlogUser Author { get; set; }

    public int PostID { get; set; }

    [Required]
    public Post Post { get; set; }

    // Could add comment status so it'd have to be confirmed by an admin before showing up on a post.
}