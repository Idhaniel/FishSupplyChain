using FishSupplyChain.Contracts.ContractFunctions;
using Microsoft.Extensions.Options;
using Nethereum.ABI.CompilationMetadata;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;

namespace FishSupplyChain.Services
{
    public class FishSupplyChainContractService
    {
        private readonly Web3 _web3;
        private readonly string _contractAddress;
        private readonly string _abi;
        private readonly BlockchainSettings settings;

        // Set up connection to blockchain
        public FishSupplyChainContractService(IOptions<BlockchainSettings> config)
        {
            settings = config.Value;
            var account = new Account(settings.PrivateKey);
            _web3 = new Web3(account, settings.RpcUrl);
            _contractAddress = settings.ContractAddress;
            var abiPath = Path.Combine(Directory.GetCurrentDirectory(), "Contracts", "FishSupplyChain.abi.json");
            _abi = File.ReadAllText(abiPath);
        }


        // Assign a role to a participant in the supply chain
        public async Task<string> AssignSupplyChainRoleAsync(string participant, int roleInt)
        {
            AssignSupplyChainRoleFunction assignSupplyChainMessage = new()
            {
                Participant = participant,
                RoleInt = roleInt
            };

            var transactionHandler = _web3.Eth.GetContractTransactionHandler<AssignSupplyChainRoleFunction>();
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10)); // Set a timeout for the transaction
            var transactionReceipt = await transactionHandler.SendRequestAndWaitForReceiptAsync(_contractAddress, assignSupplyChainMessage, cts.Token);
            if (transactionReceipt.Status.Value == 1)
            {
                Console.WriteLine("Transaction Mined Successfully");
                return transactionReceipt.TransactionHash;
            }
            Console.WriteLine("Transaction Failed");
            return transactionReceipt.TransactionHash;
        }

        //******************************Unnecessary now *******************************************//////
        //public static byte[] HexStringToByteArray(string hex)
        //{
        //    if (hex.StartsWith("0x"))
        //        hex = hex.Substring(2);

        //    if (hex.Length != 64)
        //        throw new ArgumentException("Hex string must be exactly 32 bytes (64 hex chars)");

        //    return Enumerable.Range(0, hex.Length / 2)
        //        .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
        //        .ToArray();
        //}
        //******************************Unnecessary now *******************************************//////


        // Record a shipment in the supply chain
        public async Task<string> RecordShipmentAsync(string pk, byte[] shipmentHash, decimal price)
        {
            // Connect to the blockchain using the user's private key
            var userAccount = new Account(pk);
            var userWeb3 = new Web3(userAccount, settings.RpcUrl);
            
            // Prepare the calldata
            var priceInWei = Web3.Convert.ToWei(price);
            //byte[] shipmentHashBytes = HexStringToByteArray(shipmentHashHex);
            RecordShipmentFunction recordShipmentMessage = new()
            {      
                //ShipmentHash = shipmentHashBytes,
                ShipmentHash = shipmentHash,
                Price = priceInWei
            };

            // Send the transaction
            var transactionHandler = userWeb3.Eth.GetContractTransactionHandler<RecordShipmentFunction>();
            var transactionReceipt = await transactionHandler.SendRequestAndWaitForReceiptAsync(_contractAddress, recordShipmentMessage);
            if (transactionReceipt.Status.Value == 1)
            {
                Console.WriteLine("Transaction Mined Successfully");
                return transactionReceipt.TransactionHash;
            }
            Console.WriteLine("Transaction Failed");
            return transactionReceipt.TransactionHash;
        }

        // Pay for a shipment in the supply chain
        public async Task<string> PayForShipmentAsync(string pk, int shipmentId, decimal amount)
        {
            // Connect to the blockchain using the user's private key
            var userAccount = new Account(pk);
            var userWeb3 = new Web3(userAccount, settings.RpcUrl);

            var amountInWei = Web3.Convert.ToWei(amount); // Convert to Wei if needed
            PayForShipmentFunction payForShipmentMessage = new()
            {
                ShipmentId = new BigInteger(shipmentId),
                AmountToSend = amountInWei
            };
            var transactionHandler = userWeb3.Eth.GetContractTransactionHandler<PayForShipmentFunction>();
            var transactionReceipt = await transactionHandler.SendRequestAndWaitForReceiptAsync(_contractAddress, payForShipmentMessage);
            if (transactionReceipt.Status.Value == 1)
            {
                Console.WriteLine("Transaction Mined Successfully");
                return transactionReceipt.TransactionHash;
            }
            Console.WriteLine("Transaction Failed");
            return transactionReceipt.TransactionHash;
        }

        // Get the hash of a shipment by its ID
        public async Task<byte[]> GetShipmentHashAsync(int shipmentId)
        {
            GetShipmentHashFunction getShipmentHashMessage = new()
            {
                ShipmentId = new BigInteger(shipmentId)
            };
            var queryHandler = _web3.Eth.GetContractQueryHandler<GetShipmentHashFunction>();
            var shipmentHash = await queryHandler.QueryAsync<byte[]>(_contractAddress, getShipmentHashMessage);
            return shipmentHash;
        }

        // Get a shipment by its ID
        public async Task<ShipmentDTO> GetShipmentAsync(int shipmentId)
        {
            GetShipmentFunction getShipmentMessage = new()
            {
                ShipmentId = new BigInteger(shipmentId)
            };
            var queryHandler = _web3.Eth.GetContractQueryHandler<GetShipmentFunction>();
            var shipment = await queryHandler.QueryAsync<ShipmentDTO>(_contractAddress, getShipmentMessage);
            return shipment;
        }

        // Get role of a participant in the supply chain
        public async Task<int> GetParticipantRoleAsync(string participant)
        {
            GetParticipantRoleFunction getParticipantRoleMessage = new()
            {
                Participant = participant
            };
            var queryHandler = _web3.Eth.GetContractQueryHandler<GetParticipantRoleFunction>();
            var role = await queryHandler.QueryAsync<byte>(_contractAddress, getParticipantRoleMessage);
            return role;
        }

        // Get the current shipment counter
        public async Task<BigInteger> GetShipmentCounterAsync()
        {
            GetShipmentCounterFunction getShipmentCounterMessage = new();
            var queryHandler = _web3.Eth.GetContractQueryHandler<GetShipmentCounterFunction>();
            var shipmentCounter = await queryHandler.QueryAsync<BigInteger>(_contractAddress, getShipmentCounterMessage);
            return shipmentCounter;
        }

        // Get the price of a shipment by its ID 
        public async Task<BigInteger> GetShipmentPriceAsync(int shipmentId)
        {
            GetShipmentPriceFunction getShipmentPriceMessage = new()
            {
                ShipmentId = new BigInteger(shipmentId)
            };
            var queryHandler = _web3.Eth.GetContractQueryHandler<GetShipmentPriceFunction>();
            var price = await queryHandler.QueryAsync<BigInteger>(_contractAddress, getShipmentPriceMessage);
            return price;
        }
    }
}
