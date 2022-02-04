using System.Text.Json.Serialization;

namespace ZStormBot.Constants;

public class PatchInfo
{
    [JsonIgnore]
    public string Name { get; set; }

    [JsonPropertyName("general")]
    public List<string> General { get; set; }

    [JsonPropertyName("items")]
    public Dictionary<string, List<string>> Items { get; set; }

    [JsonPropertyName("heroes")]
    public Dictionary<string, List<string>> Heroes { get; set; }
}
