using System.Text.Json.Serialization;

namespace SubscriberClient.Models;

public class LineLoginTokenResponse
{
    [JsonPropertyName("id_token")]
    public string IdToken { get; set; }
}