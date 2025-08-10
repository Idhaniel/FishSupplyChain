namespace FishSupplyChain.Entities
{
    public class FishFarmEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public ICollection<FishPondEntity> Ponds { get; set; }
    }
}
