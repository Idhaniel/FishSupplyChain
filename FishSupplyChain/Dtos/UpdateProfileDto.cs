using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    public class UpdateProfileDto
    {
        [MinLength(3, ErrorMessage = "First name should not be less than 3 characters")]
        [MaxLength(50, ErrorMessage = "First name should not be more than 50 characters")]
        public string? FirstName { get; set; }

        [MinLength(3, ErrorMessage = "Last name should not be less than 3 characters")]
        [MaxLength(50, ErrorMessage = "Last name should not be more than 50 characters")]
        public string? LastName { get; set; }
    }
}
