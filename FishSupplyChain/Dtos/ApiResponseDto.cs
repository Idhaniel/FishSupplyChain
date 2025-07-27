namespace FishSupplyChain.Dtos
{
    public abstract class ApiResponseDto
    {
        public abstract bool Success { get;}
        public abstract int StatusCode { get; init; }
    }
}
