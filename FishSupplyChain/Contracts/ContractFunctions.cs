using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace FishSupplyChain.Contracts.ContractFunctions
{
    [FunctionOutput]
    public class ShipmentDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "Id", 1)]
        public BigInteger Id { get; set; }

        [Parameter("bytes32", "ShipmentHash", 2)]
        public byte[] ShipmentHash { get; set; }

        [Parameter("uint256", "Price", 3)]
        public BigInteger Price { get; set; }

        [Parameter("address", "CreatedBy", 4)]
        public string CreatedBy { get; set; }

        [Parameter("address", "CurrentOwner", 5)]
        public string CurrentOwner { get; set; }

        [Parameter("address", "PreviousOwner", 6)]
        public string PreviousOwner { get; set; }

        [Parameter("uint8", "CurrentOwnerRole", 7)]
        public byte CurrentOwnerRole { get; set; }

        [Parameter("bool", "IsPaid", 8)]
        public bool IsPaid { get; set; }

        [Parameter("bool", "Exists", 9)]
        public bool Exists { get; set; }
    }

    [Function("assignSupplyChainRole")]
    public class AssignSupplyChainRoleFunction : FunctionMessage
    {
        [Parameter("address", "participant", 1)]
        public string Participant { get; set; }

        [Parameter("uint256", "roleInt", 2)]
        public BigInteger RoleInt { get; set; }
    }

    [Function("recordShipment")]
    public class RecordShipmentFunction : FunctionMessage
    {
        [Parameter("bytes32", "shipmentHash", 1)]
        public byte[] ShipmentHash { get; set; }

        [Parameter("uint256", "price", 2)]
        public BigInteger Price { get; set; }
    }

    [Function("payForShipment")]
    public class PayForShipmentFunction : FunctionMessage
    {
        [Parameter("uint256", "shipmentId", 1)]
        public BigInteger ShipmentId { get; set; }
    }

    [Function("getShipmentHash", "bytes32")]
    public class GetShipmentHashFunction : FunctionMessage
    {
        [Parameter("uint256", "shipmentId", 1)]
        public BigInteger ShipmentId { get; set; }
    }

    [Function("getParticipantRole", "uint8")]
    public class GetParticipantRoleFunction : FunctionMessage
    {
        [Parameter("address", "participant", 1)]
        public string Participant { get; set; }
    }

    [Function("getShipmentCounter", "uint256")]
    public class GetShipmentCounterFunction : FunctionMessage {}

    [Function("getShipmentPrice", "uint256")]
    public class GetShipmentPriceFunction : FunctionMessage
    {
        [Parameter("uint256", "shipmentId", 1)]
        public BigInteger ShipmentId { get; set; }
    }

    [Function("getShipment", typeof(ShipmentDTO))]
    public class GetShipmentFunction : FunctionMessage
    {
        [Parameter("uint256", "shipmentId", 1)]
        public BigInteger ShipmentId { get; set; }
    }
}