namespace FishSupplyChain.Services
{
    public interface IIpfsService
    {
        Task<string> UploadJsonAsync(string jsonContent, string fileName);
    }
}
