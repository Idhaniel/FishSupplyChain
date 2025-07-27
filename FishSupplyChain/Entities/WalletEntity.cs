namespace FishSupplyChain.Entities
{
    public class WalletEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }
        public string Address { get; set; }
        public string PrivateKey { get; set; }
        public string SeedPhrase { get; set; }
    }
}
