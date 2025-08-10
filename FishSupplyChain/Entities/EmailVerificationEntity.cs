namespace FishSupplyChain.Entities
{
    public class EmailVerificationEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }
        public string Token { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
