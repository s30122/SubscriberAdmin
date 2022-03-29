using System.Text.Json.Serialization;

namespace SubscriberClient.Models;

public class LineNotifyTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}