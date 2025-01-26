using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MvcBlog.Data;

public class BlogUser : IdentityUser
{
    [PersonalData]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(16, ErrorMessage = "Username can't have more than 30 characters.", MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9._-]*$")]
    public string CustomUsername { get; set; } = string.Empty;

    // public ICollection<Post> Posts { get; set; }

    // Add roles (such as Admin and Writer)
}