using FishSupplyChain.Data;
using FishSupplyChain.Dtos;
using FishSupplyChain.Entities;
using FishSupplyChain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBitcoin;
using Nethereum.HdWallet;

namespace FishSupplyChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly JwtService jwtService;
        private readonly PasswordHandlerService passwordHandler;
        private readonly FishSupplyChainDbContext dbContext;

        // Register Services via DI
        public AuthController(
            PasswordHandlerService _passwordHandler,
            FishSupplyChainDbContext _dbContext,
            JwtService _jwtService)
        {
            passwordHandler = _passwordHandler;
            dbContext = _dbContext;
            jwtService = _jwtService;
        }

        // Sign In an existing user
        [HttpPost("signin")]
        public async Task<ActionResult<ApiResponseDto>> SignIn([FromBody] SignInDto request)
        {
            // Query user details from database
            var userData = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Email == request.Email)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Password,
                    Wallet = u.Wallet.Address
                })
                .FirstOrDefaultAsync();

            // Check if user exists
            if (userData == null)
            {
                return BadRequest(new ErrorResponseDto<string> { Errors = "Invalid User", StatusCode = StatusCodes.Status400BadRequest });
            }

            // Verify passwords match
            bool verified = passwordHandler.VerifyPassword(userData.Password, request.Password);
            if (!verified)
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Invalid Password", StatusCode = StatusCodes.Status401Unauthorized });
            }

            // Generate access token
            CreateTokenForUserDto user = new(userData.Id, userData.Email);
            string token = jwtService.CreateToken(user);

            // Send Response
            AuthResponseDto response = new()
            {
                Token = token,
                User = new UserInfoDto(userData.Id, userData.FirstName, userData.LastName, userData.Email, userData.Wallet)
            };

            return Ok(new SuccessResponseDto<AuthResponseDto> { Data = response, StatusCode = StatusCodes.Status200OK });
        }

        // Sign Up a new user
        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponseDto>> SignUp([FromBody] SignUpDto request)
        {
            // Check if existing user on database
            bool userExists = await dbContext.Users.AsNoTracking().AnyAsync(user => user.Email == request.Email);

            // Reject if already exist
            if (userExists)
            {
                return Conflict(new ErrorResponseDto<string> { Errors = "User already exists", StatusCode = StatusCodes.Status409Conflict });
            }

            // Generate new wallet address to tie to user
            Mnemonic mnemo = new(Wordlist.English, WordCount.Twelve);
            string Password = "password";
            var wallet = new Wallet(mnemo.ToString(), Password);
            var account = wallet.GetAccount(0);

            // Create new user on database
            UserEntity user = new()
            {
                Email = request.Email,
                Password = passwordHandler.HashPassword(request.Password), // Hash the password first
                FirstName = request.FirstName,
                LastName = request.LastName,
                Wallet = new WalletEntity
                {
                    Address = account.Address,
                    PrivateKey = account.PrivateKey,
                    SeedPhrase = string.Join(" ", wallet.Words),
                }
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // Generate Token
            CreateTokenForUserDto userData = new(user.Id, user.Email);
            string token = jwtService.CreateToken(userData);

            // Send Response
            AuthResponseDto response = new()
            {
                Token = token,
                User = new UserInfoDto(user.Id, user.FirstName, user.LastName, user.Email, user.Wallet.Address)
            };

            return Created("", new SuccessResponseDto<AuthResponseDto> { Data = response, StatusCode = StatusCodes.Status200OK });
        }
    }
}
