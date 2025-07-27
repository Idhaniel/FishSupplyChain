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
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Provide last name")]
        public string LastName { get; set; }
    }
}
