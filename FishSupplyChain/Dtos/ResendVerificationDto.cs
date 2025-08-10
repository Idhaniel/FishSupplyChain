using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    public class ResendVerificationDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }

}
