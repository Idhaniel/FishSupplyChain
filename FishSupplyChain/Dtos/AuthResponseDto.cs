namespace FishSupplyChain.Dtos
{
    /// <summary>
    /// Response returned when user successfully signs in.
    /// Contains access token and user details.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// JWT access token used for authenticating subsequent requests.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Public information about the authenticated user.
        /// </summary>
        public UserInfoDto User {  get; set; }
    }

    /// <summary>
    /// Represents basic information about a user.
    /// </summary>
    /// <param name="Id">Unique identifier for the user.</param>
    /// <param name="FirstName">User's first name.</param>
    /// <param name="LastName">User's last name.</param>
    /// <param name="Email">User's email address.</param>
    /// <param name="Wallet">Wallet address linked to the user.</param>
    public record UserInfoDto (int Id, string FirstName, string LastName, string Email, string Wallet);
}
