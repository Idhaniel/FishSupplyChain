namespace FishSupplyChain.Entities
{
    public class PasswordResetTokenEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }
        public string TokenHash { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}
