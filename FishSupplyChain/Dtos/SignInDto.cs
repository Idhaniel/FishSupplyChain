using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    /// <summary>
    /// DTO for user sign-in request.
    /// </summary>
    public class SignInDto
    {
        /// <summary>
        /// User's email address. Must be a valid email format.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// User's account password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
