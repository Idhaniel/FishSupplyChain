using FishSupplyChain.Dtos;
using FishSupplyChain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FishSupplyChain.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService shipmentService;

        public ShipmentsController(IShipmentService _shipmentService) 
        { 
            shipmentService = _shipmentService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto>> CreateShipment([FromBody] CreateShipmentDto request)
        {
            // Check if authorized (disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            var result = await this.shipmentService.CreateShipmentAsync(userId, request);

            // Store the merged data on IPFS

            // Hash the merged data and store it on the blockchain

            // Blockchain emits event thats stored n the database
            return Created("", result);
        }
    }
}
