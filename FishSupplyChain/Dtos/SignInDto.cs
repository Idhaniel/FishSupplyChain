using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    public class SignInDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
