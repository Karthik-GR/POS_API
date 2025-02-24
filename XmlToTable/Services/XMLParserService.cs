namespace POS_API.Services
{
    using System.Net.Http;
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
                // Fetch XML content from URL
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string xmlContent = await response.Content.ReadAsStringAsync();

                // Parse XML with namespace handling
                var doc = XDocument.Parse(xmlContent);
                XNamespace xs = "http://www.w3.org/2001/XMLSchema";

                // Iterate over <xs:simpleType> elements
                foreach (var simpleType in doc.Descendants(xs + "simpleType"))
                {
                    var parentName = simpleType.Attribute("name")?.Value;
                    var parentDocumentation = simpleType
                        .Descendants(xs + "documentation")
                        .FirstOrDefault()?.Value?.Trim();

                    // Create the parent element
                    var parentElement = new ParsedElement
                    {
                        ElementName = parentName,
                        Description = parentDocumentation,
                        Children = new List<ParsedElement>()
                    };

                    // Iterate over each <xs:enumeration> inside <xs:restriction>
                    foreach (var enumeration in simpleType.Descendants(xs + "enumeration"))
                    {
                        var value = enumeration.Attribute("value")?.Value;

                        // Extract the documentation for each enumeration
                        var childDocumentation = enumeration
                            .Descendants(xs + "documentation")
                            .FirstOrDefault()?.Value?.Trim();

                        // Skip rows with missing or empty documentation
                        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(childDocumentation))
                        {
                            parentElement.Children.Add(new ParsedElement
                            {
                                ElementName = value,
                                Description = childDocumentation
                            });
                        }
                    }

                    // Add the parent element only if it has documentation or at least one child with documentation
                    if (!string.IsNullOrEmpty(parentDocumentation) || parentElement.Children.Any())
                    {
                        parsedElements.Add(parentElement);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching or parsing XML: {ex.Message}");
            }

            return parsedElements;
        }




    }
}
