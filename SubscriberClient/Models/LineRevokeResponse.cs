using System.Net;
using System.Text.Json.Serialization;

namespace SubscriberClient.Models;

public class LineRevokeResponse
{
    [JsonPropertyName("status")]
    public HttpStatusCode Status { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}