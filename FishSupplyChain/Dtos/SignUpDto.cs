using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    public class SignUpDto
    {
        [EmailAddress(ErrorMessage = "Provide a valid email")]
        [Required(ErrorMessage = "Provide an email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Provide a password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Provide first name")]
        [MinLength(3, ErrorMessage = "First name should not be less than 3 characters")]
        [MaxLength(50, ErrorMessage = "First name should not be more than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Provide last name")]
        [MinLength(3, ErrorMessage = "Last name should not be less than 3 characters")]
        [MaxLength(50, ErrorMessage = "Last name should not be more than 50 characters")]
        public string LastName { get; set; }
    }
}
