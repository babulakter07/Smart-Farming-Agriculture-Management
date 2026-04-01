using System.ComponentModel.DataAnnotations;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Application.DTOs;

public class UserListDto
{
    public string Id { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<string> AssignedFarms { get; set; } = [];
}

public class UserCreateDto
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = null!;
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required, MinLength(8)]
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.Viewer;
    public List<int> FarmIds { get; set; } = [];
}

public class UserEditDto
{
    public string Id { get; set; } = null!;
    [Required, MaxLength(200)]
    public string FullName { get; set; } = null!;
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public List<int> FarmIds { get; set; } = [];
}

public class LoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = null!;
    [Required, MinLength(8)]
    public string NewPassword { get; set; } = null!;
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = null!;
}
