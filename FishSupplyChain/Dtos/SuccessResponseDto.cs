namespace FishSupplyChain.Dtos
{
    public sealed class SuccessResponseDto<TData> : ApiResponseDto
    {
        public override bool Success => true;
        public TData Data { get; set; }
        public override int StatusCode { get; init; }
    }
}
