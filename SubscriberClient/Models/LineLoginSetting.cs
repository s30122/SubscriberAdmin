namespace SubscriberClient.Models;

public class LineLoginSetting
{
    public string ClientId { get; set; }
    public string RedirectUri { get; set; }
    public string State { get; set; }
    public string Secret { get; set; }
}