using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EnergyManager.Client;
using System.Text;
using System.Text.Json;

namespace EnergyManager.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        [BindProperty]
        public IFormFile? Upload { get; set; }

        [BindProperty]
        public ClientResponse? ReadingResponse { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async Task OnPostAsync()
        {
            HttpClient client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7268/meter-reading-uploads");
            request.Content = new StringContent(GetFileContents(), Encoding.UTF8, "text/csv");
            
            var response = client.Send(request);
            var responseData = await response.Content.ReadAsStringAsync();
            ReadingResponse = JsonSerializer.Deserialize<ClientResponse>(responseData);
        }

        private string GetFileContents()
        {
            if (Upload != null)
            {
                using (var stream = Upload.OpenReadStream())
                {
                    string fileContents;
                    using (var reader = new StreamReader(stream))
                    {
                        fileContents = reader.ReadToEnd();
                        return fileContents;
                    }
                }
            }
            return string.Empty;
        }
    }
}