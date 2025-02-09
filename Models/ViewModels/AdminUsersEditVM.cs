using Microsoft.AspNetCore.Identity;
using MvcBlog.Data;

namespace MvcBlog.Models.ViewModels;

public class AdminUsersEditVM
{
    public string ID { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public IEnumerable<string>? SelectedRoles { get; set; }

    public IEnumerable<IdentityRole>? Roles { get; set; }
}