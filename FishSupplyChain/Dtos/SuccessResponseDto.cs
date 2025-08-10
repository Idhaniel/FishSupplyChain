namespace FishSupplyChain.Dtos
{
    /// <summary>
    /// Standard wrapper for successful API responses.
    /// </summary>
    /// <typeparam name="TData">Type of data returned in the response.</typeparam>
    public sealed class SuccessResponseDto<TData> : ApiResponseDto
    {
        /// <summary>
        /// Indicates that the request was successful.
        /// Always true for this DTO.
        /// </summary>
        public override bool Success => true;

        /// <summary>
        /// The actual data returned from the request.
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// HTTP status code of the response.
        /// </summary>
        public override int StatusCode { get; init; }
    }
}
