using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
