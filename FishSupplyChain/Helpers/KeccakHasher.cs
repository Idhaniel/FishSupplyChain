using Nethereum.Util;
using System.Text;

namespace FishSupplyChain.Helpers
{
    public static class KeccakHasher
    {
        /// <summary>
        /// Computes a Keccak-256 hash from a string and returns raw bytes (ready for Solidity bytes32).
        /// </summary>
        public static byte[] Keccak256Bytes(string input)
        {
            var keccak = new Sha3Keccack();
            return keccak.CalculateHash(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Computes a Keccak-256 hash from a byte array and returns raw bytes.
        /// </summary>
        public static byte[] Keccak256Bytes(byte[] input)
        {
            var keccak = new Sha3Keccack();
            return keccak.CalculateHash(input);
        }

        /// <summary>
        /// Computes a Keccak-256 hash from a string and returns it as a lowercase hex string (without 0x).
        /// </summary>
        public static string Keccak256Hex(string input)
        {
            var bytes = Keccak256Bytes(input);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
