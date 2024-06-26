using System.Text.Json;

namespace BotSharp.Abstraction.Functions.Models;

public class FunctionParametersDef
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    /// <summary>
    /// ParameterPropertyDef
    /// {
    ///     "field_name": {}
    /// }
    /// </summary>
    [JsonPropertyName("properties")]
    public JsonDocument Properties { get; set; } = JsonSerializer.Deserialize<JsonDocument>("{}");

    [JsonPropertyName("required")]
    public List<string> Required {  get; set; } = new List<string>();

    public override string ToString()
    {
        return $"{{\"type\":\"{Type}\", \"properties\":{JsonSerializer.Serialize(Properties)}, \"required\":[{string.Join(",", Required.Select(x => "\"" + x + "\""))}]}}";
    }

    public FunctionParametersDef()
    {
        
    }
}
