using FishSupplyChain.Dtos;

namespace FishSupplyChain.Services
{
    public interface IShipmentService
    {
        Task<string> CreateShipmentAsync(int userId, CreateShipmentDto shipment);
    }
}

