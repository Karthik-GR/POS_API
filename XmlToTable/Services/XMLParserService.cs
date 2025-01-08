namespace POS_API.Services
{
    using System.Net.Http;
    using System.Text;
    using System.Xml.Linq;
    using POS_API.Model;

    public class XMLParserService
    {
        private readonly HttpClient _httpClient;

        public XMLParserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ParsedElement>> ParseXmlAsync(string url)
        {
            var parsedElements = new List<ParsedElement>();

            try
            {
                // Fetch XML from URL
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string xmlContent = await response.Content.ReadAsStringAsync();

                // Parse XML
                var doc = XDocument.Parse(xmlContent);
                var elements = doc.Descendants()
                    .Where(e => e.Attribute("name") != null || e.Attribute("value") != null);

                foreach (var element in elements)
                {
                    string elementValue = element.Attribute("name")?.Value ?? element.Attribute("value")?.Value;
                    string documentation = element.Descendants("{http://www.w3.org/2001/XMLSchema}documentation")
                                                  .FirstOrDefault()?.Value?.Trim();

                    if (!string.IsNullOrEmpty(documentation))
                    {
                        parsedElements.Add(new ParsedElement
                        {
                            ElementName = elementValue,
                            Description = documentation
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (logging can be added)
                throw new Exception($"Error fetching or parsing XML: {ex.Message}");
            }

            return parsedElements;
        }
    }

}
