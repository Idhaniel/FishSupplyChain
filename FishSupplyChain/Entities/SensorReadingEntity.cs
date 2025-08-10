namespace FishSupplyChain.Entities
{
    public class SensorReadingEntity
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public SensorEntity Sensor { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double PHLevel { get; set; }
        public double OxygenLevel { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
