using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
