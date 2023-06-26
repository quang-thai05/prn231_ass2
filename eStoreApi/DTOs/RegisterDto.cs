using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.DTOs;

[NotMapped]
public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Password and confirmation password not match")]
    public string ConfirmPassword { get; set; }
}