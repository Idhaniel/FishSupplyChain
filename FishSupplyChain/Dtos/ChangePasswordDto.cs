using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm new password is required")]
        [Compare(nameof(NewPassword), ErrorMessage = "New password and confirm new password do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}
