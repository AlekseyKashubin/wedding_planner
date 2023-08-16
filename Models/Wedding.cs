#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Add this using statement to access NotMapped
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace wedding_planner.Models;
public class Wedding
{
    [Key]
    public int WeddingId { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Grooms name must be at least 2 characters")]
    public string Groom { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Brides name must be at least 2 characters")]
    public string Bride { get; set; }
    
    
    [Required (ErrorMessage = "Must Add a Date")]
    [FutureDate]
    [DataType(DataType.Date)]
    [Display(Name = "Wedding Date")]
    public DateTime WeddingDate { get; set; }


    [Required]
    public string Address { get; set; }



    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


    // one to many
    public int UserId { get; set; }
    public User? Creator { get; set; }



// Many to many example

public List<RSVP> RSVPers { get; set; } = new List<RSVP>();

}





public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        DateTime Now = DateTime.Now;
        DateTime Input = (DateTime)value;

        if (Input<Now)
        {
            return new ValidationResult("Date Must Be In The Future");
        }else
        {
            return ValidationResult.Success;
        }
    }
}