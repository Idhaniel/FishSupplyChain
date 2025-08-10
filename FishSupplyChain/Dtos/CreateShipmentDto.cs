namespace FishSupplyChain.Dtos
{
    public class CreateShipmentDto
    {
        public int PondId { get; set; }              // Selected from farmer's ponds list
        public DateTime StartTime { get; set; }      // Start of the sensor logs time range
        public DateTime EndTime { get; set; }        // End of the sensor logs time range
        public decimal Price { get; set; }           // Price for on-chain payment / transfer
        public string? Notes { get; set; }           // Optional notes like "harvested at sunrise"
        public DateTime HarvestTime { get; set; }    // Explicit harvest time
    }
}
