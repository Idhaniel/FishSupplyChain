using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace FishSupplyChain.Contracts.ContractEvents
{
    [Event("ParticipantAssignedRole")]
    public class ParticipantAssignedRoleEventDTO : IEventDTO
    {
        [Parameter("address", "Participant", 1, true)]
        public string Participant { get; set; }

        [Parameter("uint8", "Role", 2, true)] // Enum mapped to uint8
        public byte Role { get; set; }
    }

    [Event("ShipmentRecorded")]
    public class ShipmentRecordedEventDTO : IEventDTO
    {
        [Parameter("uint256", "ShipmentId", 1, true)]
        public BigInteger ShipmentId { get; set; }

        [Parameter("address", "CreatedBy", 2, true)]
        public string CreatedBy { get; set; }

        [Parameter("bytes32", "ShipmentHash", 3, true)]
        public byte[] ShipmentHash { get; set; }

        [Parameter("uint256", "Price", 4, false)]
        public BigInteger Price { get; set; }
    }

    [Event("ShipmentPaidAndTransferred")]
    public class ShipmentPaidAndTransferredEventDTO : IEventDTO
    {
        [Parameter("uint256", "ShipmentId", 1, true)]
        public BigInteger ShipmentId { get; set; }

        [Parameter("address", "NewOwner", 2, true)]
        public string NewOwner { get; set; }

        [Parameter("address", "OldOwner", 3, true)]
        public string OldOwner { get; set; }

        [Parameter("uint256", "Price", 4, false)]
        public BigInteger Price { get; set; }
    }

}
