using FishSupplyChain.Data;
using FishSupplyChain.Dtos;
using FishSupplyChain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Security.Claims;

namespace FishSupplyChain.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly FishSupplyChainContractService contractService;
        private readonly FishSupplyChainDbContext dbContext;

        public ContractController(FishSupplyChainContractService _contractService, FishSupplyChainDbContext _dbContext)
        {
            contractService = _contractService;
            dbContext = _dbContext;
        }

        // Roles should be assigned on sign up already and not here. NB, roles are permanent in this system.
        [HttpPost("assign")]
        public async Task<ActionResult<ApiResponseDto>> AssignRole([FromBody] AssignRoleDto request)
        {
            // Check if authorized (disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            // Take the user's address from the database
            string? address = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.Wallet.Address)
                .FirstOrDefaultAsync();

            if (address is null)
            {
                return NotFound(new ErrorResponseDto<string> { Errors = "User address not found", StatusCode = StatusCodes.Status404NotFound });
            }

            string hash = await contractService.AssignSupplyChainRoleAsync(address , (int)request.Role);
            return Ok(new SuccessResponseDto<string> { StatusCode = StatusCodes.Status200OK, Data = hash });
        }

        //******************************Redundant Was used in Dev******************///
        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);

            if (hex.Length != 64)
                throw new ArgumentException("Hex string must be exactly 32 bytes (64 hex chars)");

            return [.. Enumerable.Range(0, hex.Length / 2).Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))];
        }
        //******************************Redundant Was used in Dev******************///

        // Record shipment
        [HttpPost("record")]
        public async Task<ActionResult<ApiResponseDto>> RecordShipment([FromBody] RecordShipmentDto request)
        {
            // Check if authorized(disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            // Take the user's address from the database
            string? pk = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.Wallet.PrivateKey)
                .FirstOrDefaultAsync();
            if (pk is null)
            {
                return NotFound(new ErrorResponseDto<string> { Errors = "User pk not found", StatusCode = StatusCodes.Status404NotFound });
            }
            Console.WriteLine(pk);
            byte[] shipmentHash = HexStringToByteArray(request.ShipmentHash);
            string hash = await contractService.RecordShipmentAsync(pk, shipmentHash, request.Price);
            return Ok(new SuccessResponseDto<string> { StatusCode = StatusCodes.Status200OK, Data = hash });
        }

        // Pay for shipment
        [HttpPost("pay")]
        public async Task<ActionResult<ApiResponseDto>> PayForShipment([FromBody] PayForShipmentDto request)
        {
            // Check if authorized(disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            // Take the user's address from the database
            string? pk = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.Wallet.PrivateKey)
                .FirstOrDefaultAsync();
            if (pk is null)
            {
                return NotFound(new ErrorResponseDto<string> { Errors = "User pk not found", StatusCode = StatusCodes.Status404NotFound });
            }
            Console.WriteLine(pk);

            string hash = await contractService.PayForShipmentAsync(pk, request.ShipmentId, request.Amount);
            return Ok(new SuccessResponseDto<string> { StatusCode = StatusCodes.Status200OK, Data = hash });
        }

        // Get count of shipment
        [HttpGet("shipment")]
        public async Task<ActionResult<ApiResponseDto>> GetShipmentCounter()
        {
            // Check if authorized(disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            var shipmentCount = await contractService.GetShipmentCounterAsync();
            return Ok(new SuccessResponseDto<BigInteger> { StatusCode = StatusCodes.Status200OK, Data = shipmentCount });
        }

        // Get hash of shipment
        [HttpGet("shipment/hash/{id:int}")]
        public async Task<ActionResult<ApiResponseDto>> GetShipmentHash([FromRoute] int id)
        {
            // Check if authorized(disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            var shipmentHash = await contractService.GetShipmentHashAsync(id);
            return Ok(new SuccessResponseDto<byte[]> { StatusCode = StatusCodes.Status200OK, Data = shipmentHash });
        }

        // Get price of shipment
        [HttpGet("shipment/price/{id:int}")]
        public async Task<ActionResult<ApiResponseDto>> GetShipmentPrice([FromRoute] int id)
        {
            // Check if authorized(disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            var shipmentPrice = await contractService.GetShipmentPriceAsync(id);
            return Ok(new SuccessResponseDto<BigInteger> { StatusCode = StatusCodes.Status200OK, Data = shipmentPrice });
        }

        // Get shipment
        [HttpGet("shipment/{id:int}")]
        public async Task<ActionResult<ApiResponseDto>> GetShipment([FromRoute] int id)
        {
            // Check if authorized(disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            var shipment = await contractService.GetShipmentAsync(id);
            return Ok(new SuccessResponseDto<object> { StatusCode = StatusCodes.Status200OK, Data = shipment });
        }

        // Get role of participant
        [HttpGet("role/{address}")]
        public async Task<ActionResult<ApiResponseDto>> GetParticipantRole([FromRoute] string address)
        {
            // Check if authorized(disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            var role = await contractService.GetParticipantRoleAsync(address);
            return Ok(new SuccessResponseDto<int> { StatusCode = StatusCodes.Status200OK, Data = role });
        }
    }
}
