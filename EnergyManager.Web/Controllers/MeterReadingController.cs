using Microsoft.AspNetCore.Mvc;
using EnergyManager.Data.Models;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace EnergyManager.Web.Controllers
{
    [ApiController]
    [Route("/meter-reading-uploads")]
    public class MeterReadingController : ControllerBase
    {
        private readonly ILogger<MeterReadingController> _logger;

        public MeterReadingController(ILogger<MeterReadingController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Consumes("text/csv")]
        public async Task<IActionResult> OnPost()
        {
            int numberOfValidRecords = 0;
            int numberOfInvalidRecords = 0;
            Stream stream = Request.Body;
            using (StreamReader streamReader = new StreamReader(stream))
            {

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower()
                };

                using (var csv = new CsvReader(streamReader, config))
                {
                    await csv.ReadAsync();
                    csv.ReadHeader();
                    while (await csv.ReadAsync())
                    {
                        var record = csv.GetRecord<RawMeterReading>();
                        // First check that there is only data in three fields
                        if (record.Valid())
                        {
                            numberOfValidRecords++;
                        }
                        else
                        {
                            numberOfInvalidRecords++;
                        }
                    }
                }
            }

            return new JsonResult(new Dictionary<string, int>() { {"numberOfValidRecords", numberOfValidRecords},
                                                                  {"numberOfInvalidRecords", numberOfInvalidRecords} });
        }

        [HttpGet]
        public async Task<IActionResult> OnGet()
        {
            return await Task.Run(() => new JsonResult(Data.Models.MeterReading.GetAll()));
        }
    }
}