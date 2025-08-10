namespace FishSupplyChain.Dtos
{
    /// <summary>
    /// Standard wrapper for failed API responses.
    /// </summary>
    /// <typeparam name="TErr">Type of error details.</typeparam>
    public sealed class ErrorResponseDto<TErr> : ApiResponseDto
    {
        /// <summary>
        /// Indicates that the request failed.
        /// Always false for this DTO.
        /// </summary>
        public override bool Success => false;

        /// <summary>
        /// One or more error messages describing why the request failed.
        /// </summary>
        public TErr Errors { get; init; }

        /// <summary>
        /// HTTP status code of the response.
        /// </summary>
        public override int StatusCode { get; init; }
    }
}
