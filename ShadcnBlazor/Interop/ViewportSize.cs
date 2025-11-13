using System.Text.Json.Serialization;

namespace ShadcnBlazor.Interop;

public class ViewportSize
{
    [JsonPropertyName("height")]
    public double Height { get; set; }
    
    [JsonPropertyName("width")]
    public double Width { get; set; }
}