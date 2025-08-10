namespace FishSupplyChain.Entities
{
    public class RefreshTokenEntity
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; } 
        public UserEntity User { get; set; }   
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? CreatedByIp { get; set; }
    }

}

