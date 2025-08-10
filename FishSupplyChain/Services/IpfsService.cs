using FishSupplyChain.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FishSupplyChain.Services
{
    public class IpfsService : IIpfsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pinataJwt;
        private readonly string _localIpfsApi;

        public IpfsService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _pinataJwt = configuration["IPFSCredentials:PinataJwt"];
            _localIpfsApi = configuration["IPFSCredentials:LocalApi"] ?? "http://localhost:5001/api/v0/add?pin=true";
        }

        public async Task<string> UploadJsonAsync(string jsonContent, string fileName)
        {
            // Upload JSON to local IPFS and get CID
            //var cid = await AddToLocalIpfs(jsonContent, fileName);

            // Pin same CID to Pinata
            //await PinCidToPinata(cid);

            var cid = await UploadJsonToPinata(jsonContent, fileName);
            return cid;

            //return cid;
        }

        private async Task<string> AddToLocalIpfs(string jsonContent, string fileName)
        {
            using var form = new MultipartFormDataContent();
            var jsonBytes = Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(jsonBytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //form.Add(byteContent, "file", fileName);
            form.Add(byteContent, "file");

            var response = await _httpClient.PostAsync(_localIpfsApi, form);
            var responseText = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseText);
            using var doc = JsonDocument.Parse(responseText);
            return doc.RootElement.GetProperty("Hash").GetString(); // This is the CID
        }

        private async Task PinCidToPinata(string cid)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.pinata.cloud/pinning/pinByHash");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _pinataJwt);

            var payload = new
            {
                hashToPin = cid,
                pinataMetadata = new { name = $"Pinned_{cid}" }
            };

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> UploadJsonToPinata(string jsonContent, string fileName)
        {
            using var form = new MultipartFormDataContent();
            var jsonBytes = Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(jsonBytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            form.Add(byteContent, "file", fileName);

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.pinata.cloud/pinning/pinFileToIPFS");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _pinataJwt);
            request.Content = form;

            var response = await _httpClient.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseText);
            return doc.RootElement.GetProperty("IpfsHash").GetString(); // CID
        }
    }
}


