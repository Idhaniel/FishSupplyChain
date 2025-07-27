namespace FishSupplyChain.Dtos
{
    public class ErrorResponseDto<TErr> : ApiResponseDto
    {
        public override bool Success => false;
        public TErr Errors { get; init; }
        public override int StatusCode { get; init; }
    }
}
