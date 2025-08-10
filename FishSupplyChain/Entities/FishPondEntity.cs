namespace FishSupplyChain.Entities
{
    public class FishPondEntity
    {
        public int Id { get; set; }
        public int FishFarmId { get; set; }
        public FishFarmEntity FishFarm { get; set; }
        public string Name { get; set; }
        public SensorEntity Sensor { get; set; }
    }
}
