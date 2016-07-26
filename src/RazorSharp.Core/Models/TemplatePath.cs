namespace RazorSharp.Core.Models
{
    using Newtonsoft.Json;

    public class TemplatePath : ITemplateContainer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("outputPath")]
        public string OutputPath { get; set; }

        [JsonProperty("items")]
        public TemplateItem[] Items { get; set; }

        [JsonProperty("paths")]
        public TemplatePath[] Paths { get; set; }
    }
}