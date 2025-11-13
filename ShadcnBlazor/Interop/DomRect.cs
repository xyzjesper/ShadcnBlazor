using System.Text.Json.Serialization;

namespace ShadcnBlazor.Interop;

public class DomRect
{
    [JsonPropertyName("height")] public double Height { get; set; }
    [JsonPropertyName("width")] public double Width { get; set; }
    [JsonPropertyName("x")] public double X { get; set; }
    [JsonPropertyName("y")] public double Y { get; set; }
    
    [JsonPropertyName("left")] public double Left { get; set; }
    [JsonPropertyName("right")] public double Right { get; set; }
    [JsonPropertyName("top")] public double Top { get; set; }
    [JsonPropertyName("bottom")] public double Bottom { get; set; }
}