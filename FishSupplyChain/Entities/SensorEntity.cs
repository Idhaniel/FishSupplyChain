namespace FishSupplyChain.Entities
{
    public class SensorEntity
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public FishPondEntity Pond { get; set; }
        public ICollection<SensorReadingEntity> Readings { get; set; }
    }
}
