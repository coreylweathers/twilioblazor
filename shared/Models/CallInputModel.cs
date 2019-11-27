using System.ComponentModel.DataAnnotations;

namespace shared.Models
{
    public class CallInputModel
    {
        [Required]
        [Phone(ErrorMessage = "Please enter your phone number in a proper format")]
        public string PhoneNumber { get; set; }
    }
}
