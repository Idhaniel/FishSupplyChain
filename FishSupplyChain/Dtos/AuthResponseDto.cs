namespace FishSupplyChain.Dtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public UserInfoDto User {  get; set; }
    }

    public record UserInfoDto (int Id, string FirstName, string LastName, string Email, string Wallet);
}
