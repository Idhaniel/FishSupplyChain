using System.ComponentModel.DataAnnotations;

namespace FishSupplyChain.Dtos
{
    public class SensorReadingDto
    {
        [Required]
        public int SensorId { get; set; }
        [Required]
        public double Temperature { get; set; }
        [Required]
        public double Humidity { get; set; }
        [Required]
        public double PHLevel { get; set; }
        [Required]
        public double OxygenLevel { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
