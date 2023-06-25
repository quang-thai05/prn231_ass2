using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BussinessObject;

public class User : IdentityUser
{
    public ICollection<Order>? Orders { get; set; }
}