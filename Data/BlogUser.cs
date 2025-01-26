using Microsoft.AspNetCore.Identity;

namespace MvcBlog.Data;

public class BlogUser : IdentityUser
{
    [PersonalData]
    public string Name { get; set; } = string.Empty;

    // Add roles (such as Admin and Writer)
}