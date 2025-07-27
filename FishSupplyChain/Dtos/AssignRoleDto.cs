using FishSupplyChain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    public class AssignRoleDto
    {
        // Add validation
        [Required]
        [EnumDataType(typeof(FishSupplyChainRole))]
        [Range((int)FishSupplyChainRole.Unassigned, (int)FishSupplyChainRole.Retailer,
            ErrorMessage = "Role must be one of: Unassigned, Farmer, Processor, Distributor, Retailer")]
        public FishSupplyChainRole Role { get; set; }
    }
}
