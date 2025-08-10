using FishSupplyChain.Data;
using FishSupplyChain.Dtos;
using FishSupplyChain.Helpers;
using FishSupplyChain.Types.CustomExceptionTypes;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;
using System.Text.Json;

namespace FishSupplyChain.Services
{
    public class ShipmentService(
        FishSupplyChainDbContext dbContext, 
        IIpfsService ipfsService,
        FishSupplyChainContractService contractService) : IShipmentService
    {
        public async Task<string> CreateShipmentAsync(int userId, CreateShipmentDto request)
        {
            // Validate pond ownership
            var pond = await dbContext.Ponds
                .Include(p => p.FishFarm)
                .FirstOrDefaultAsync(p => p.Id == request.PondId && p.FishFarm.UserId == userId);

            if (pond == null)
            {
                throw new NotFoundException("Pond not found or not owned by this user.");
            }

            // Take the user's pk from the database
            string? pk = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.Wallet.PrivateKey)
                .FirstOrDefaultAsync();
            if (pk is null)
            {
                throw new NotFoundException("User pk not found");
            }
            Console.WriteLine(pk);

            // Query sensor logs
            var logs = await dbContext.SensorReadings
                //.Include(r => r.Sensor)
                .Where(r => r.Sensor.PondId == request.PondId &&
                            r.Timestamp >= request.StartTime &&
                            r.Timestamp <= request.EndTime)
                .Select(r => new
                {
                    r.Temperature,
                    r.Humidity,
                    r.PHLevel,
                    r.OxygenLevel,
                    r.Timestamp
                })
                .ToListAsync();

            //var sensor = await dbContext.Sensors
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(s => s.PondId == request.PondId);

            //if (sensor == null)
            //    throw new BadRequestException("No sensors were found for this pond");

            //var logs = await dbContext.SensorReadings
            //    .AsNoTracking()
            //    .Where(r => r.SensorId == sensor.Id && // Filter by the single Sensor's ID
            //                r.Timestamp >= request.StartTime &&
            //                r.Timestamp <= request.EndTime)
            //    .Select(r => new
            //    {
            //        r.Temperature,
            //        r.Humidity,
            //        r.PHLevel,
            //        r.OxygenLevel,
            //        r.Timestamp
            //    })
            //    .ToListAsync();

            // Build shipment JSON
            var shipmentData = new
            {
                ShipmentMeta = new
                {
                    FarmName = pond.FishFarm.Name,
                    request.HarvestTime,
                    request.Price,
                    request.Notes
                },
                SensorLogs = logs
            };
            var json = JsonSerializer.Serialize(shipmentData);
            Console.WriteLine(json);

            // Upload to IPFS
            var fileName = request.HarvestTime.ToString().Replace("/", "_").Replace(":", "-");
            string cid = await ipfsService.UploadJsonAsync(json, $"Shipment_{fileName}.json");

            Console.WriteLine($"IPFS done: Heres the result: {cid}");

            // Hash the data and sha make it compatible with solidity bytes32
            Console.WriteLine();
            Console.WriteLine("Starting to store on chain");
            byte[] shipmentHash = KeccakHasher.Keccak256Bytes(json);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(KeccakHasher.Keccak256Hex(json));
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Store on chain, wait for event to store something on DB
            string hash = await contractService.RecordShipmentAsync(pk, shipmentHash, request.Price);

            return hash;
        }
    }
}
