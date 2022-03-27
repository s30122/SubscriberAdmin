using System.Text.Json.Serialization;

namespace SubscriberClient.Models;

public class LineLoginVerifyResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}