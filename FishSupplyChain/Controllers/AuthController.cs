using FishSupplyChain.Data;
using FishSupplyChain.Dtos;
using FishSupplyChain.Entities;
using FishSupplyChain.Helpers;
using FishSupplyChain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBitcoin;
using Nethereum.Contracts.Standards.ERC20.TokenList;
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
        private readonly IEmailService emailService;

        // Register Services via DI
        public AuthController(
            PasswordHandlerService _passwordHandler,
            FishSupplyChainDbContext _dbContext,
            JwtService _jwtService,
            IEmailService _emailService
            )
        {
            passwordHandler = _passwordHandler;
            dbContext = _dbContext;
            jwtService = _jwtService;
            emailService = _emailService;
        }

        // Sign In an existing user
        /// <summary>
        /// Authenticates a user and returns access and refresh tokens.
        /// </summary>
        /// <remarks>
        /// This endpoint allows a registered and verified user to sign in by providing valid credentials.  
        /// If successful, it returns an access token and attaches a refresh token as an HttpOnly cookie.  
        /// If the account is unverified or credentials are invalid, appropriate error responses are returned.
        ///
        /// Sample request:
        /// 
        ///     POST /signin
        ///     {
        ///         "email": "jane.doe@example.com",
        ///         "password": "YourStrongPassword123"
        ///     }
        ///
        /// Sample successful response:
        /// 
        ///     {
        ///         "success": true,
        ///         "data": {
        ///             "token": "eyJhbGciOiJIUzI1NiIsInR...",
        ///             "user": {
        ///                 "id": 1,
        ///                 "firstName": "Jane",
        ///                 "lastName": "Doe",
        ///                 "email": "jane.doe@example.com",
        ///                 "wallet": "0xabc123..."
        ///             }
        ///         },
        ///         "statusCode": 200
        ///     }
        ///
        /// Sample error response (unverified):
        /// 
        ///     {
        ///         "success": false,
        ///         "errors": "User not verified",
        ///         "statusCode": 401
        ///     }
        /// </remarks>
        /// <param name="request">User login credentials (email and password).</param>
        /// <returns>A success response with access token and user info, or an error if authentication fails.</returns>
        [HttpPost("signin")]
        [ProducesResponseType(typeof(SuccessResponseDto<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto<string>), StatusCodes.Status401Unauthorized)]
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
                    u.IsVerified,
                    u.Password,
                    Wallet = u.Wallet.Address
                })
                .FirstOrDefaultAsync();

            // Check if user exists
            if (userData == null)
            {
                return BadRequest(new ErrorResponseDto<string> { Errors = "Invalid User", StatusCode = StatusCodes.Status400BadRequest });
            }

            // Check if user is verified
            if (!userData.IsVerified)
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "User not verified", StatusCode = StatusCodes.Status401Unauthorized });
            }

            // Verify passwords match
            bool verified = passwordHandler.VerifyPassword(userData.Password, request.Password);
            if (!verified)
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Invalid Password", StatusCode = StatusCodes.Status401Unauthorized });
            }

            // Generate access and refresh token
            CreateTokenForUserDto user = new(userData.Id, userData.Email);
            (string accessToken, RefreshTokenEntity refreshToken) = jwtService.GenerateTokens(user, HttpContext.Connection.RemoteIpAddress?.ToString());

            // Revoke any existing refresh tokens for this user
            var existingToken = await dbContext.RefreshTokens
                                .FirstOrDefaultAsync(rt => rt.UserId == userData.Id && !rt.IsRevoked);

            if (existingToken != null)
            {
                existingToken.IsRevoked = true;
                existingToken.ReplacedByToken = refreshToken.Token; // Clear replaced token     
            }

            dbContext.RefreshTokens.Add(refreshToken);
            await dbContext.SaveChangesAsync();

            // Attach refresh token to cookie
            Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.ExpiresAt,
                Path = "/"
            });

            // Send Response
            AuthResponseDto response = new()
            {
                Token = accessToken,
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

            // Create new user on database
            UserEntity user = new()
            {
                Email = request.Email,
                Password = passwordHandler.HashPassword(request.Password), // Hash the password first
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsVerified = false,
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            string verificationToken = TokenGenerator.GenerateSecureToken();
            string encodedToken = Uri.EscapeDataString(verificationToken);
            string verificationUrl = $"http://localhost:5137/verify-email?token={encodedToken}";

            await emailService.SendAsync(user.Email, "Verify your email", $"Click here to verify: {verificationUrl}");

            // Store email verification token in DB
            dbContext.EmailVerifications.Add(new()
            {
                UserId = user.Id,
                Token = verificationToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsUsed = false
            });
            await dbContext.SaveChangesAsync();

            // Send Response
            string response = "Sign-up successful. Please check your email to verify your account.";
            return Created("", new SuccessResponseDto<string> { Data = response, StatusCode = StatusCodes.Status201Created });
        }

        // Verify user email
        [HttpGet("verify-email")]
        public async Task<ActionResult<ApiResponseDto>> VerifyEmail([FromQuery]string token)
        {
            // Check if token is valid
            var verification = await dbContext.EmailVerifications
                .FirstOrDefaultAsync(ev => ev.Token == token && !ev.IsUsed && ev.ExpiresAt > DateTime.UtcNow);
            if (verification == null)
            {
                return BadRequest(new ErrorResponseDto<string> { Errors = "Invalid or expired verification token", StatusCode = StatusCodes.Status400BadRequest });
            }

            // Mark the token as used
            verification.IsUsed = true;
            await dbContext.SaveChangesAsync();

            // Update user as verified
            var user = await dbContext.Users.FindAsync(verification.UserId);
            if (user != null && !user.IsVerified)
            {
                user.IsVerified = true;

                // Generate new wallet address to tie to user
                Mnemonic mnemo = new(Wordlist.English, WordCount.Twelve);
                var wallet = new Wallet(mnemo.ToString(), "password");
                var account = wallet.GetAccount(0);

                user.Wallet = new ()
                {
                    Address = account.Address,
                    PrivateKey = account.PrivateKey,
                    SeedPhrase = string.Join(" ", wallet.Words),
                };
                await dbContext.SaveChangesAsync();
            }
            return Ok(new SuccessResponseDto<string> { Data = "Email verified successfully", StatusCode = StatusCodes.Status200OK });
        }

        // Refresh access token using refresh token
        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponseDto>> RefreshAccessToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Refresh token missing", StatusCode = StatusCodes.Status401Unauthorized });
            }

            var existingToken = await dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (existingToken == null || existingToken.ExpiresAt < DateTime.UtcNow || existingToken.IsRevoked)
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Invalid or expired refresh token", StatusCode = StatusCodes.Status401Unauthorized });
            }

            // Renew access token
            string accessToken = jwtService.RenewAccessToken(new CreateTokenForUserDto(existingToken.User.Id, existingToken.User.Email));

            return Ok(new SuccessResponseDto<string>
            {
                Data = accessToken,
                StatusCode = StatusCodes.Status200OK
            });
        }

        [HttpGet("logout")]
        public async Task<ActionResult<ApiResponseDto>> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "Refresh token missing", StatusCode = StatusCodes.Status401Unauthorized });
            }
            var existingToken = await dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (existingToken != null)
            {
                existingToken.IsRevoked = true;
                await dbContext.SaveChangesAsync();
            }
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/refresh-token",
                Expires = DateTime.UtcNow.AddDays(-1) // ensures full deletion
            });
            return Ok(new SuccessResponseDto<string> { Data = "Logged out successfully", StatusCode = StatusCodes.Status200OK });
        }

        [HttpPost("resend-verification")]
        public async Task<ActionResult<ApiResponseDto>> ResendVerification([FromBody] ResendVerificationDto request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound(new ErrorResponseDto<string>
                {
                    Errors = "User not found",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            if (user.IsVerified)
            {
                return BadRequest(new ErrorResponseDto<string>
                {
                    Errors = "Email already verified",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            // Mark any existing tokens as used
            var oldTokens = await dbContext.EmailVerifications
                .Where(ev => ev.UserId == user.Id && !ev.IsUsed)
                .ToListAsync();
            foreach (var old in oldTokens)
            {
                old.IsUsed = true;
            }

            // Create a new token
            string verificationToken = TokenGenerator.GenerateSecureToken();
            string encodedToken = Uri.EscapeDataString(verificationToken);
            string verificationUrl = $"http://localhost:5137/verify-email?token={encodedToken}";

            await emailService.SendAsync(user.Email, "Verify your email", $"Click here to verify: {verificationUrl}");

            dbContext.EmailVerifications.Add(new EmailVerificationEntity
            {
                UserId = user.Id,
                Token = verificationToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                IsUsed = false
            });
            await dbContext.SaveChangesAsync();

            return Ok(new SuccessResponseDto<string>
            {
                Data = "Verification email resent successfully",
                StatusCode = StatusCodes.Status200OK
            });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponseDto>> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            // Check if existing user on database
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Email == request.Email)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.IsVerified,
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new ErrorResponseDto<string> { Errors = "User not found", StatusCode = StatusCodes.Status404NotFound });
            }
            if (!user.IsVerified)
            {
                return Unauthorized(new ErrorResponseDto<string> { Errors = "User Account not verified", StatusCode = StatusCodes.Status401Unauthorized });
            }

            string passwordResetToken = TokenGenerator.GenerateSecureToken();
            string encodedToken = Uri.EscapeDataString(passwordResetToken);
            string passwordResetUrl = $"http://localhost:5137/reset-password?token={encodedToken}";

            await emailService.SendAsync(user.Email, "Reset your password", $"A request to reset your password was initiated: {passwordResetUrl}. If this was not you. Please ignore this mail");

            // Store email verification token in DB
            dbContext.PasswordResetTokens.Add(new()
            {
                UserId = user.Id,
                TokenHash = TokenHasher.HashToken(passwordResetToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                IsUsed = false
            });
            await dbContext.SaveChangesAsync();

            return Ok(new SuccessResponseDto<string>
            {
                Data = "A link to reset your password has been sent to your mail",
                StatusCode = StatusCodes.Status200OK
            });
        }
     
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(SuccessResponseDto<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto<string>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponseDto>> ResetPassword([FromBody] ResetPasswordDto request)
        {
            string hashedToken = TokenHasher.HashToken(request.Token);

            var resetTokenEntry = await dbContext.PasswordResetTokens
                .FirstOrDefaultAsync(t =>
                    t.TokenHash == hashedToken &&
                    t.ExpiresAt > DateTime.UtcNow &&
                    !t.IsUsed);

            if (resetTokenEntry is null)
            {
                return BadRequest(new ErrorResponseDto<string>
                {
                    Errors = "Invalid or expired reset token.",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var user = await dbContext.Users.FindAsync(resetTokenEntry.UserId);
            if (user is null)
            {
                return NotFound(new ErrorResponseDto<string>
                {
                    Errors = "User not found.",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            // Update password
            user.Password = passwordHandler.HashPassword(request.NewPassword);
            resetTokenEntry.IsUsed = true;
            await dbContext.SaveChangesAsync();

            return Ok(new SuccessResponseDto<string>
            {
                Data = "Password reset successful.",
                StatusCode = StatusCodes.Status200OK
            });
        }


    }
}
