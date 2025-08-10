using FishSupplyChain.Data;
using FishSupplyChain.Dtos;
using FishSupplyChain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FishSupplyChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly FishSupplyChainDbContext dbContext;
        private readonly PasswordHandlerService passwordHandlerService;
        public UsersController(FishSupplyChainDbContext _dbContext, PasswordHandlerService _passwordHandlerService)
        {
            dbContext = _dbContext;
            passwordHandlerService = _passwordHandlerService;
        }

        [HttpPatch("profile")]
        [ProducesResponseType(typeof(SuccessResponseDto<UserInfoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponseDto<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto<Dictionary<string, string[]>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponseDto>> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            // Check if authorized (disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }

            var user =  await dbContext.Users.FindAsync(userId);
            if (user is null)
            {
                return NotFound(new ErrorResponseDto<string>
                {
                    Errors = "User not found",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            bool updated = false;

            if (!string.IsNullOrWhiteSpace(request.FirstName) && request.FirstName != user.FirstName)
            {
                user.FirstName = request.FirstName;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.LastName) && request.LastName != user.LastName)
            {
                user.LastName = request.LastName;
                updated = true;
            }

            if (updated)
            {
                await dbContext.SaveChangesAsync();
            }

            var updatedUser = new UserInfoDto(user.Id, user.FirstName, user.LastName, user.Email, user.Wallet.Address);

            return Ok(new SuccessResponseDto<UserInfoDto>
            {
                Data = updatedUser,
                StatusCode = StatusCodes.Status200OK
            });
        }

        [HttpPatch("change-password")]
        [ProducesResponseType(typeof(SuccessResponseDto<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponseDto<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponseDto>> ChangePassword([FromBody] ChangePasswordDto request)
        {
            // Check if authorized (disallow if not)
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (subClaim is null || !int.TryParse(subClaim.Value, out int userId))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Missing or invalid token claim", StatusCode = StatusCodes.Status401Unauthorized });
            }
            var user = await dbContext.Users.FindAsync(userId);
            if (user is null)
            {
                return NotFound(new ErrorResponseDto<string>
                {
                    Errors = "User not found",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            
            if (passwordHandlerService.VerifyPassword(user.Password, request.CurrentPassword))
            {
                return BadRequest(new ErrorResponseDto<string>
                {
                    Errors = "Incorrect Password",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            user.Password = passwordHandlerService.HashPassword(request.NewPassword);
            await dbContext.SaveChangesAsync();

            return Ok(new SuccessResponseDto<string>
            {
                Data = "Password changed successfully",
                StatusCode = StatusCodes.Status200OK
            });
        }
    }
}
