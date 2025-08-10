using FishSupplyChain.Data;
using FishSupplyChain.Dtos;
using HiveMQtt.Client;
using HiveMQtt.MQTT5.Types;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FishSupplyChain.Services
{
    public class HiveMqttService : BackgroundService
    {
        private HiveMQClient client;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly HiveMQqtSettings hiveMQqtSettings;

        public HiveMqttService(IServiceScopeFactory _scopeFactory, IOptions<HiveMQqtSettings> _hiveMQqtSettings)
        {
            scopeFactory = _scopeFactory;
            hiveMQqtSettings = _hiveMQqtSettings.Value;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var options = new HiveMQClientOptionsBuilder()
                .WithBroker(hiveMQqtSettings.Broker)
                .WithPort(hiveMQqtSettings.Port)
                .WithUseTls(true)
                .WithUserName(hiveMQqtSettings.Username)
                .WithPassword(hiveMQqtSettings.Password)
                .Build();

            client = new HiveMQClient(options);

            client.OnMessageReceived += async (sender, args) =>
            {
                var message = args.PublishMessage.PayloadAsString;
                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("Empty payload received.");
                    return;
                }

                SensorReadingDto? data;
                try
                {
                    data = JsonSerializer.Deserialize<SensorReadingDto>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Invalid JSON: {ex.Message}");
                    return; // Stop processing
                }

                if (data is null ||
                    data.SensorId <= 0 ||
                    data.Temperature <= 0 ||
                    data.Humidity <= 0 ||
                    data.PHLevel <= 0 ||
                    data.OxygenLevel <= 0 ||
                    data.Timestamp == default)
                {
                    Console.WriteLine("Invalid or incomplete sensor data received.");
                    return;
                }

                // Save the message to the database
                Console.WriteLine($"Received message from Sensor {data.SensorId} at {data.Timestamp}");
                Console.WriteLine($"Received message: {data}");

                // Create a scope to access the database context
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<FishSupplyChainDbContext>();
                
                dbContext.SensorReadings.Add(new()
                {
                    SensorId = data.SensorId,
                    Temperature = data.Temperature,
                    Humidity = data.Humidity,
                    PHLevel = data.PHLevel,
                    OxygenLevel = data.OxygenLevel,
                    Timestamp = data.Timestamp
                });

                await dbContext.SaveChangesAsync();
            };

            // Connect to the MQTT broker
            var connectResult = await client.ConnectAsync().ConfigureAwait(false);

            // Configure the subscriptions we want and subscribe
            var builder = new SubscribeOptionsBuilder();
            builder.WithSubscription("topic1", QualityOfService.AtLeastOnceDelivery)
                   .WithSubscription("topic2", QualityOfService.ExactlyOnceDelivery);
            var subscribeOptions = builder.Build();
            var subscribeResult = await client.SubscribeAsync(subscribeOptions);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (client.IsConnected() || client != null)
            {
                await client.DisconnectAsync();
            }
            await base.StopAsync(cancellationToken);
        }
    }
}





// Publish a message
//var publishResult = await client.PublishAsync("topic1/example", "This is the server");