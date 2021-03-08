namespace ReadABit.Core.Integrations.Services
{
    public static class SparvPipelineService
    {
        public static SparvXmlAnnotation ToSparvXml(string twoLetterISOLanguageName, string input)
        {
            return new SparvXmlAnnotation
            {
                XmlJson = "// TODO: ",
                Version = "// TODO: ",
            };
        }
    }

    public record SparvXmlAnnotation
    {
        public string XmlJson { get; init; } = "";
        public string Version { get; init; } = "";
    }
}
