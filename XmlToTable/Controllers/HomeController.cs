using Microsoft.AspNetCore.Mvc;
using POS_API.Services;

namespace XmlToTable.Controllers
{
    public class HomeController : Controller
    {
        private readonly XMLParserService _xmlParserService;

        public HomeController(XMLParserService xmlParserService)
        {
            _xmlParserService = xmlParserService;
        }

        public async Task<IActionResult> Index(string url)
        {
            // Default URL if none is passed
            url ??= "https://receiptservice.egretail.cloud/ARTSPOSLogSchema/6.0.0";

            var parsedElements = await _xmlParserService.ParseXmlAsync(url);
            return View(parsedElements);
        }
    }
}
