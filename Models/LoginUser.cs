#pragma warning disable CS8618
// using statements and namespace go here
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace wedding_planner.Models;

[NotMapped]
public class LoginUser
{
    [Required]
    [Display(Name ="Email Address")]
    public string LoginEmail { get; set; }
    [Required]
    [Display(Name ="Password")]
    [DataType(DataType.Password)]
    public string LoginPassword { get; set; }
}
